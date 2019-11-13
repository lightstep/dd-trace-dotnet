using System;
using System.Collections;
using System.Collections.Generic;
using Datadog.Trace.ClrProfiler.Emit;
using Datadog.Trace.ClrProfiler.ExtensionMethods;
using Datadog.Trace.Headers;
using Datadog.Trace.Logging;

namespace Datadog.Trace.ClrProfiler.Integrations
{
    /// <summary>
    /// The ASP.NET Core MVC 3 integration.
    /// </summary>
    public static class AspNetCoreMvc3Integration
    {
        private const string HttpContextKey = "__Datadog.Trace.ClrProfiler.Integrations." + nameof(AspNetCoreMvc3Integration);
        private const string IntegrationName = "AspNetCoreMvc3";
        private const string OperationName = "aspnet-coremvc.request";
        private const string AspnetMvcCore = "Microsoft.AspNetCore.Mvc";
        private const string Major3 = "3";

        /// <summary>
        /// Type for unobtrusive hooking into Microsoft.AspNetCore.Mvc.Core pipeline.
        /// </summary>
        private const string DiagnosticSourceTypeName = "Microsoft.AspNetCore.Mvc.MvcCoreDiagnosticListenerExtensions";

        /// <summary>
        /// Base type used for traversing the pipeline in Microsoft.AspNetCore.Mvc.Core.
        /// </summary>
        private const string ResourceInvokerTypeName = "Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker";

        private static readonly Vendors.Serilog.ILogger Log = DatadogLogging.GetLogger(typeof(AspNetCoreMvc2Integration));

        private static AspNetCoreMvc3Context CreateContext(object actionDescriptor, object httpContext)
        {
            var context = new AspNetCoreMvc3Context();

            try
            {
                var request = httpContext.GetProperty("Request").GetValueOrDefault();

                GetTagValues(
                    actionDescriptor,
                    request,
                    out string httpMethod,
                    out string host,
                    out string resourceName,
                    out string url,
                    out string controllerName,
                    out string actionName);

                SpanContext propagatedContext = null;
                var tracer = Tracer.Instance;

                if (tracer.ActiveScope == null)
                {
                    try
                    {
                        // extract propagated http headers
                        var requestHeaders = request.GetProperty<IEnumerable>("Headers").GetValueOrDefault();

                        if (requestHeaders != null)
                        {
                            var headersCollection = new DictionaryHeadersCollection();

                            foreach (object header in requestHeaders)
                            {
                                var key = header.GetProperty<string>("Key").GetValueOrDefault();
                                var values = header.GetProperty<IList<string>>("Value").GetValueOrDefault();

                                if (key != null && values != null)
                                {
                                    headersCollection.Add(key, values);
                                }
                            }

                            propagatedContext = SpanContextPropagator.Instance.Extract(headersCollection);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error extracting propagated HTTP headers.");
                    }
                }

                var scope = tracer.StartActive(OperationName, propagatedContext);
                context.Scope = scope;
                var span = scope.Span;

                span.DecorateWebServerSpan(
                    resourceName: resourceName,
                    method: httpMethod,
                    host: host,
                    httpUrl: url);

                span.SetTag(Tags.AspNetController, controllerName);
                span.SetTag(Tags.AspNetAction, actionName);

                var analyticsSampleRate = tracer.Settings.GetIntegrationAnalyticsSampleRate(IntegrationName, enabledWithGlobalSetting: true);
                span.SetMetric(Tags.Analytics, analyticsSampleRate);
            }
            catch (Exception ex)
            {
                context.SafeToContinue = false;
                context.Scope.Dispose();
                Log.Error(
                    ex,
                    "An exception occurred when trying to initialize a Scope for {0}",
                    nameof(AspNetCoreMvc3Integration));
                throw;
            }

            return context;
        }

        /// <summary>
        /// Wrapper method used to instrument Microsoft.AspNetCore.Mvc.Internal.MvcCoreDiagnosticSourceExtensions.BeforeAction()
        /// </summary>
        /// <param name="instance">The DiagnosticSource that this extension method was called on.</param>
        /// <param name="httpContext">The HttpContext for the current request.</param>
        /// <param name="actionDescriptor">The shut up style cop.</param>
        /// <param name="opCode">The OpCode used in the original method call.</param>
        /// <param name="mdToken">The mdToken of the original method call.</param>
        /// <param name="moduleVersionPtr">A pointer to the module version GUID.</param>
        [InterceptMethod(
            CallerAssembly = AspnetMvcCore,
            TargetAssembly = AspnetMvcCore,
            TargetType = DiagnosticSourceTypeName,
            TargetSignatureTypes = new[] { ClrNames.Void, ClrNames.Ignore, ClrNames.Ignore },
            TargetMinimumVersion = "1",
            TargetMaximumVersion = "6")]
        public static void BeforeOnResourceExecuted(
            object instance,
            object httpContext,
            object actionDescriptor,
            int opCode,
            int mdToken,
            long moduleVersionPtr)
        {
            AspNetCoreMvc3Context context = null;

            if (!Tracer.Instance.Settings.IsIntegrationEnabled(IntegrationName))
            {
                // integration disabled
                return;
            }

            try
            {
                context = CreateContext(actionDescriptor, httpContext);

                if (context.SafeToContinue
                 && httpContext.TryGetPropertyValue("Items", out IDictionary<object, object> contextItems))
                {
                    contextItems[HttpContextKey] = context;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error creating {nameof(AspNetCoreMvc3Context)}.");
            }

            Action<object, object> instrumentedMethod = null;

            try
            {
                instrumentedMethod =
                    MethodBuilder<Action<object, object>>
                       .Start(moduleVersionPtr, mdToken, opCode, nameof(BeforeOnResourceExecuted))
                       .WithConcreteType(null)
                       .WithParameters(actionDescriptor, httpContext)
                       .WithNamespaceAndNameFilters(
                            ClrNames.Void,
                            ClrNames.Ignore,
                            ClrNames.Ignore)
                       .Build();
            }
            catch (Exception ex)
            {
                Log.ErrorRetrievingMethod(
                    exception: ex,
                    moduleVersionPointer: moduleVersionPtr,
                    mdToken: mdToken,
                    opCode: opCode,
                    instrumentedType: DiagnosticSourceTypeName,
                    methodName: nameof(BeforeOnResourceExecuted),
                    instanceType: null,
                    relevantArguments: new[] { "TBD" });
                throw;
            }

            try
            {
                // call the original method, catching and rethrowing any unhandled exceptions
                instrumentedMethod?.Invoke(actionDescriptor, httpContext);
            }
            catch (Exception ex)
            {
                context?.Scope?.Span?.SetException(ex);
                throw;
            }
        }

        /// <summary>
        /// Wrapper method used to instrument Microsoft.AspNetCore.Mvc.Internal.MvcCoreDiagnosticSourceExtensions.AfterAction()
        /// </summary>
        /// <param name="instance">An ActionDescriptor with information about the current action.</param>
        /// <param name="httpContext">The HttpContext for the current request.</param>
        /// <param name="opCode">The OpCode used in the original method call.</param>
        /// <param name="mdToken">The mdToken of the original method call.</param>
        /// <param name="moduleVersionPtr">A pointer to the module version GUID.</param>
        [InterceptMethod(
            CallerAssembly = AspnetMvcCore,
            TargetAssembly = AspnetMvcCore,
            TargetType = "TBD",
            TargetSignatureTypes = new[] { ClrNames.Void, ClrNames.Ignore },
            TargetMinimumVersion = "1",
            TargetMaximumVersion = "6")]
        public static void AfterOnResourceExecuted(
            object instance,
            object httpContext,
            int opCode,
            int mdToken,
            long moduleVersionPtr)
        {
            AspNetCoreMvc3Context context = null;

            if (!Tracer.Instance.Settings.IsIntegrationEnabled(IntegrationName))
            {
                // integration disabled
                return;
            }

            try
            {
                if (httpContext.TryGetPropertyValue("Items", out IDictionary<object, object> contextItems))
                {
                    context = contextItems?[HttpContextKey] as AspNetCoreMvc3Context;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error accessing {nameof(AspNetCoreMvc2Integration)}.");
            }

            Action<object> instrumentedMethod = null;

            try
            {
                instrumentedMethod =
                    MethodBuilder<Action<object>>
                       .Start(moduleVersionPtr, mdToken, opCode, nameof(AfterOnResourceExecuted))
                       .WithConcreteType(null)
                       .WithParameters(httpContext)
                       .Build();
            }
            catch (Exception ex)
            {
                Log.ErrorRetrievingMethod(
                    exception: ex,
                    moduleVersionPointer: moduleVersionPtr,
                    mdToken: mdToken,
                    opCode: opCode,
                    instrumentedType: DiagnosticSourceTypeName,
                    methodName: nameof(AfterOnResourceExecuted),
                    instanceType: null,
                    relevantArguments: new[] { instance?.GetType().AssemblyQualifiedName });
                throw;
            }

            try
            {
                // call the original method, catching and rethrowing any unhandled exceptions
                instrumentedMethod.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                context?.Scope?.Span?.SetException(ex);
                throw;
            }
            finally
            {
                context?.Scope?.Dispose();
            }
        }

        private static void GetTagValues(
            object actionDescriptor,
            object request,
            out string httpMethod,
            out string host,
            out string resourceName,
            out string url,
            out string controllerName,
            out string actionName)
        {
            controllerName = actionDescriptor.GetProperty<string>("ControllerName").GetValueOrDefault()?.ToLowerInvariant();

            actionName = actionDescriptor.GetProperty<string>("ActionName").GetValueOrDefault()?.ToLowerInvariant();

            host = request.GetProperty("Host").GetProperty<string>("Value").GetValueOrDefault();

            httpMethod = request.GetProperty<string>("Method").GetValueOrDefault()?.ToUpperInvariant() ?? "UNKNOWN";

            string pathBase = request.GetProperty("PathBase").GetProperty<string>("Value").GetValueOrDefault();

            string path = request.GetProperty("Path").GetProperty<string>("Value").GetValueOrDefault();

            string queryString = request.GetProperty("QueryString").GetProperty<string>("Value").GetValueOrDefault();

            url = $"{pathBase}{path}{queryString}";

            string resourceUrl = actionDescriptor.GetProperty("AttributeRouteInfo").GetProperty<string>("Template").GetValueOrDefault() ??
                                 UriHelpers.GetRelativeUrl(new Uri($"https://{host}{url}"), tryRemoveIds: true).ToLowerInvariant();

            resourceName = $"{httpMethod} {resourceUrl}";
        }

        private class AspNetCoreMvc3Context
        {
            public bool SafeToContinue { get; set; } = true;

            public Scope Scope { get; set; }
        }
    }
}
