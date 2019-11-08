using System.Collections.Generic;
using System.Linq;
using Datadog.Trace.TestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Datadog.Trace.ClrProfiler.IntegrationTests.AWS
{
    public class AmazonSqsTests : TestHelper
    {
        private readonly List<AmazonSqsExpectation> _synchronousExpectations = new List<AmazonSqsExpectation>();
        private readonly List<AmazonSqsExpectation> _asynchronousExpectations = new List<AmazonSqsExpectation>();
        private readonly List<AmazonSqsExpectation> _expectations = new List<AmazonSqsExpectation>();

        public AmazonSqsTests(ITestOutputHelper output)
            : base("Amazon.Sqs", output)
        {
            _synchronousExpectations.Add(CreateExpectation("CreateQueue"));
            _synchronousExpectations.Add(CreateExpectation("ListQueues"));
            _synchronousExpectations.Add(CreateExpectation("GetQueueUrl"));
            _synchronousExpectations.Add(CreateExpectation("SendMessage"));
            _synchronousExpectations.Add(CreateExpectation("SendMessage"));
            _synchronousExpectations.Add(CreateExpectation("ReceiveMessage"));
            _synchronousExpectations.Add(CreateExpectation("DeleteMessage"));
            _synchronousExpectations.Add(CreateExpectation("SendMessageBatch"));
            _synchronousExpectations.Add(CreateExpectation("SendMessageBatch"));
            _synchronousExpectations.Add(CreateExpectation("ReceiveMessage"));
            _synchronousExpectations.Add(CreateExpectation("DeleteMessageBatch"));
            _synchronousExpectations.Add(CreateExpectation("PurgeQueue"));
            _synchronousExpectations.Add(CreateExpectation("DeleteQueue"));

            _asynchronousExpectations.Add(CreateExpectation("CreateQueue"));
            _asynchronousExpectations.Add(CreateExpectation("ListQueues"));
            _asynchronousExpectations.Add(CreateExpectation("GetQueueUrl"));
            _asynchronousExpectations.Add(CreateExpectation("SendMessage"));
            _asynchronousExpectations.Add(CreateExpectation("SendMessage"));
            _asynchronousExpectations.Add(CreateExpectation("ReceiveMessage"));
            _asynchronousExpectations.Add(CreateExpectation("DeleteMessage"));
            _asynchronousExpectations.Add(CreateExpectation("SendMessageBatch"));
            _asynchronousExpectations.Add(CreateExpectation("SendMessageBatch"));
            _asynchronousExpectations.Add(CreateExpectation("ReceiveMessage"));
            _asynchronousExpectations.Add(CreateExpectation("DeleteMessageBatch"));
            _asynchronousExpectations.Add(CreateExpectation("PurgeQueue"));
            _asynchronousExpectations.Add(CreateExpectation("DeleteQueue"));

            _expectations.AddRange(_asynchronousExpectations);
#if NETFRAMEWORK
            _expectations.AddRange(_synchronousExpectations);
#endif
        }

        [Theory]
        [MemberData(nameof(PackageVersions.AmazonSqs), MemberType = typeof(PackageVersions))]
        [Trait("Category", "EndToEnd")]
        public void SubmitsTraces(string packageVersion)
        {
            int agentPort = TcpPortProvider.GetOpenPort();
            using (var agent = new MockTracerAgent(agentPort))
            using (var processResult = RunSampleAndWaitForExit(agent.Port, packageVersion: packageVersion))
            {
                Assert.True(processResult.ExitCode >= 0, $"Process exited with code {processResult.ExitCode} and exception: {processResult.StandardError}");

                var spans = agent.WaitForSpans(_expectations.Count, 500, operationName: AmazonExpectation.IntegrationOperationName);
                Assert.True(spans.Count >= _expectations.Count, $"Expecting at least {_expectations.Count} spans, only received {spans.Count}");

                SpanTestHelpers.AssertExpectationsMet(_expectations, spans.ToList());
            }
        }

        private static AmazonSqsExpectation CreateExpectation(string awsOperation)
        {
            return new AmazonSqsExpectation("Samples.Amazon.Sqs-aws")
            {
                AwsOperation = awsOperation,
                ResourceName = $"SQS.{awsOperation}",
            };
        }
    }
}
