using Xunit;
using Xunit.Abstractions;

namespace Datadog.Trace.ClrProfiler.IntegrationTests.AspNetCore
{
    public class AspNetCoreMvc3Tests : AspNetCoreMvcTestBase
    {
        public AspNetCoreMvc3Tests(ITestOutputHelper output)
            : base("AspNetCoreMvc.Netcore3", output)
        {
        }

#if NETCOREAPP3
        [Fact]
        [Trait("Category", "EndToEnd")]
        [Trait("RunOnWindows", "True")]
        public void MeetsAllAspNetCoreMvcExpectations()
        {
            // No package versions are relevant because this is built-in
            RunTraceTestOnSelfHosted(null);
        }
#endif
    }
}
