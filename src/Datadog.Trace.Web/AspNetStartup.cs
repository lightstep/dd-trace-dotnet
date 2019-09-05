using System.Web;
using Datadog.Trace.ClrProfiler.Integrations;
using Datadog.Trace.TracingHttpModule;

[assembly: PreApplicationStartMethod(typeof(AspNetStartup), "Register")]

namespace Datadog.Trace.TracingHttpModule
{
    /// <summary>
    ///     Used as the target of a PreApplicationStartMethodAttribute on the assembly to load the AspNetHttpModule into the pipeline
    /// </summary>
    public static class AspNetStartup
    {
        /// <summary>
        ///     Registers the AspNetHttpModule at ASP.NET startup into the pipeline
        /// </summary>
        public static void Register()
        {
            Tracer.Instance = new Tracer(null, null, null, new AspNetScopeManager());

            if (Tracer.Instance.Settings.IsIntegrationEnabled(AspNetHttpModule.IntegrationName))
            {
                // only register http module if integration is enabled
                HttpApplication.RegisterModule(typeof(AspNetHttpModule));
            }
        }
    }
}
