using Datadog.Trace.ClrProfiler.IntegrationTests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Datadog.Trace.ClrProfiler.IntegrationTests.AspNetCore
{
    public class AspNetCoreMvc30Tests : AspNetCoreMvcTestBase
    {
        public AspNetCoreMvc30Tests(ITestOutputHelper output)
            : base("AspNetCoreMvc30", output)
        {
            // EnableDebugMode();
        }

        [TargetFrameworkVersionsFact("netcoreapp3.0")]
        [Trait("Category", "EndToEnd")]
        [Trait("RunOnWindows", "True")]
        public void MeetsAllAspNetCoreMvcExpectations()
        {
            // No package versions are relevant because this is built-in
            RunTraceTestOnSelfHosted(string.Empty);
        }
    }
}
