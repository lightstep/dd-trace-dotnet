using System;
using Datadog.Trace.ClrProfiler.Emit;
using Datadog.Trace.Logging;

namespace Datadog.Trace.ClrProfiler.Integrations
{
    /// <summary>
    /// Tracing integration for AWSSDK.SQS.
    /// </summary>
    public static class AWSSDKSQSIntegration
    {
        private const string IntegrationName = "AWS";
        private const string OperationName = "aws.command";

        private const string Major3 = "3";
        private const string Major3Minor3 = "3.3";

        private const string ServiceName = "aws";
        private const string AmazonSQSAssemblyName = "AWSSDK.SQS";
        private const string IAmazonSqsTypeName = "Amazon.SQS.IAmazonSQS";
        private const string SendMessageRequestTypeName = "Amazon.SQS.Model.SendMessageRequest";
        private const string SendMessageResponseTypeName = "Amazon.SQS.Model.SendMessageResponse";

        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// Wrap the original method by adding instrumentation code around it.
        /// </summary>
        /// <param name="sqs">The instance of AmazonSQS.IAmazonSQS.</param>
        /// <param name="sendMessageRequest">The message request object.</param>
        /// <param name="opCode">The OpCode used in the original method call.</param>
        /// <param name="mdToken">The mdToken of the original method call.</param>
        /// <param name="moduleVersionPtr">A pointer to the module version GUID.</param>
        /// <returns>The original method's return value.</returns>
        [InterceptMethod(
            TargetAssembly = AmazonSQSAssemblyName,
            TargetType = IAmazonSqsTypeName,
            TargetMethod = "SendMessage",
            TargetSignatureTypes = new[] { SendMessageResponseTypeName, SendMessageRequestTypeName },
            TargetMinimumVersion = Major3Minor3,
            TargetMaximumVersion = Major3)]
        public static object SendMessage(
            object sqs,
            object sendMessageRequest,
            int opCode,
            int mdToken,
            long moduleVersionPtr)
        {
            Func<object, object, object> instrumentedMethod;
            var sqsType = sqs.GetType();

            try
            {
                instrumentedMethod =
                    MethodBuilder<Func<object, object, object>>
                    .Start(moduleVersionPtr, mdToken, opCode, nameof(SendMessage))
                    .WithConcreteType(sqsType)
                    .WithParameters(sendMessageRequest)
                    .WithNamespaceAndNameFilters(SendMessageResponseTypeName, SendMessageRequestTypeName)
                    .Build();
            }
            catch (Exception ex)
            {
                Log.ErrorRetrievingMethod(
                    exception: ex,
                    moduleVersionPointer: moduleVersionPtr,
                    mdToken: mdToken,
                    opCode: opCode,
                    instrumentedType: IAmazonSqsTypeName,
                    methodName: nameof(SendMessage),
                    instanceType: sqs.GetType().AssemblyQualifiedName);
                throw;
            }

            using (var scope = CreateScopeFromSendMessage(sendMessageRequest.GetProperty<string>("QueueUrl").GetValueOrDefault()))
            {
                try
                {
                    return instrumentedMethod(sqs, sendMessageRequest);
                }
                catch (Exception ex)
                {
                    scope?.Span.SetException(ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Wrap the original method by adding instrumentation code around it.
        /// </summary>
        /// <param name="sqs">The instance of AmazonSQS.IAmazonSQS.</param>
        /// <param name="queueUrl">The URL for the queue.</param>
        /// <param name="messageBody">The body of the message.</param>
        /// <param name="opCode">The OpCode used in the original method call.</param>
        /// <param name="mdToken">The mdToken of the original method call.</param>
        /// <param name="moduleVersionPtr">A pointer to the module version GUID.</param>
        /// <returns>The original method's return value.</returns>
        [InterceptMethod(
            TargetAssembly = AmazonSQSAssemblyName,
            TargetType = IAmazonSqsTypeName,
            TargetMethod = "SendMessage",
            TargetSignatureTypes = new[] { SendMessageResponseTypeName, ClrNames.String, ClrNames.String },
            TargetMinimumVersion = Major3Minor3,
            TargetMaximumVersion = Major3)]
        public static object SendMessageOnlyStrings(
            object sqs,
            string queueUrl,
            string messageBody,
            int opCode,
            int mdToken,
            long moduleVersionPtr)
        {
            Func<object, object, object, object> instrumentedMethod;
            var sqsType = sqs.GetType();

            try
            {
                instrumentedMethod =
                    MethodBuilder<Func<object, object, object, object>>
                    .Start(moduleVersionPtr, mdToken, opCode, nameof(SendMessage))
                    .WithConcreteType(sqsType)
                    .WithParameters(queueUrl, messageBody)
                    .WithNamespaceAndNameFilters(SendMessageResponseTypeName, ClrNames.String, ClrNames.String)
                    .Build();
            }
            catch (Exception ex)
            {
                Log.ErrorRetrievingMethod(
                    exception: ex,
                    moduleVersionPointer: moduleVersionPtr,
                    mdToken: mdToken,
                    opCode: opCode,
                    instrumentedType: IAmazonSqsTypeName,
                    methodName: nameof(SendMessage),
                    instanceType: sqs.GetType().AssemblyQualifiedName);
                throw;
            }

            using (var scope = CreateScopeFromSendMessage(queueUrl))
            {
                try
                {
                    return instrumentedMethod(sqs, queueUrl, messageBody);
                }
                catch (Exception ex)
                {
                    scope?.Span.SetException(ex);
                    throw;
                }
            }
        }

        private static Scope CreateScopeFromSendMessage(string queueUrl)
        {
            if (!Tracer.Instance.Settings.IsIntegrationEnabled(IntegrationName))
            {
                // integration disabled, don't create a scope, skip this trace
                return null;
            }

            Tracer tracer = Tracer.Instance;
            Scope scope = null;
            string serviceName = string.Join("-", tracer.DefaultServiceName, ServiceName);

            try
            {
                scope = Tracer.Instance.StartActive(OperationName, serviceName: serviceName);
                var span = scope.Span;
                span.SetTag("aws.queue.url", queueUrl);

                // set analytics sample rate if enabled
                var analyticsSampleRate = tracer.Settings.GetIntegrationAnalyticsSampleRate(IntegrationName, enabledWithGlobalSetting: false);
                span.SetMetric(Tags.Analytics, analyticsSampleRate);
            }
            catch (Exception ex)
            {
                Log.ErrorException("Error creating or populating scope.", ex);
            }

            return scope;
        }
    }
}
