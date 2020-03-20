using Xunit;
using Xunit.Abstractions;

namespace Datadog.Trace.ClrProfiler.IntegrationTests.SmokeTests
{
    public class HttpHandlerStackOverflowExceptionSmokeTest : SmokeTestBase
    {
        public HttpHandlerStackOverflowExceptionSmokeTest(ITestOutputHelper output)
            : base(output, "HttpMessageHandler.StackOverflowException", maxTestRunSeconds: 15)
        {
        }

        [Fact]
        [Trait("Category", "Smoke")]
        public void NoExceptions()
        {
            CheckForSmoke(shouldDeserializeTraces: false);
        }
    }
}
