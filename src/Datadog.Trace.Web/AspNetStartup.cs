using System.Web;
using Datadog.Trace.Web;

[assembly: PreApplicationStartMethod(typeof(AspNetStartup), nameof(AspNetStartup.Register))]

namespace Datadog.Trace.Web
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
            var scopeManager = new AspNetScopeManager();

            Tracer.Instance = new Tracer(settings: null, agentWriter: null, sampler: null, scopeManager);

            if (Tracer.Instance.Settings.IsIntegrationEnabled(AspNetHttpModule.IntegrationName))
            {
                // only register http module if integration is enabled
                HttpApplication.RegisterModule(typeof(AspNetHttpModule));
            }
        }
    }
}
