using System.Web;
using Datadog.Trace.Web;

[assembly: PreApplicationStartMethod(typeof(Startup), nameof(Startup.Register))]

namespace Datadog.Trace.Web
{
    /// <summary>
    ///     Used as the target of a PreApplicationStartMethodAttribute on the assembly to load the TracingHttpModule into the pipeline
    /// </summary>
    public static class Startup
    {
        /// <summary>
        ///     Registers the TracingHttpModule at ASP.NET startup into the pipeline
        /// </summary>
        public static void Register()
        {
            var scopeManager = new AspNetScopeManager();

            Tracer.Instance = new Tracer(settings: null, agentWriter: null, sampler: null, scopeManager);

            if (Tracer.Instance.Settings.IsIntegrationEnabled(TracingHttpModule.IntegrationName))
            {
                // only register http module if integration is enabled
                HttpApplication.RegisterModule(typeof(TracingHttpModule));
            }
        }
    }
}
