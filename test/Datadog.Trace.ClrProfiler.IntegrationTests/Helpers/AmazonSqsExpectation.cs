using System.Collections.Generic;
using Datadog.Trace.ClrProfiler.Integrations;
using Datadog.Trace.TestHelpers;

namespace Datadog.Trace.ClrProfiler.IntegrationTests
{
    public class AmazonSqsExpectation : AmazonExpectation
    {
        public AmazonSqsExpectation(string serviceName)
        : base(serviceName)
        {
            TagShouldExist(AmazonTags.OperationName, Always);
            TagShouldExist(AmazonTags.AgentName, Always);
            TagShouldExist(AmazonTags.ServiceName, Always);
            TagShouldExist(AmazonTags.RequestId, Always);

            var creatingOrDeletingQueue = new HashSet<string> { Commands.CreateQueueRequest };
            TagShouldExist(AmazonTags.SqsQueueName, when: span => creatingOrDeletingQueue.Contains(GetTag(span, AmazonTags.OperationName)));

            var operationsAgainstQueue = new HashSet<string>
            {
                Commands.PurgeQueueRequest,
                Commands.SendMessageRequest,
                Commands.SendMessageBatchRequest,
                Commands.DeleteMessageRequest,
                Commands.DeleteMessageBatchRequest
            };
            TagShouldExist(AmazonTags.SqsQueueUrl, when: span => operationsAgainstQueue.Contains(GetTag(span, AmazonTags.OperationName)));

            IsTopLevel = false;
        }

        public string AwsOperation { get; set; }

        public override bool ShouldInspect(MockTracerAgent.Span span)
        {
            return
                GetTag(span, AmazonTags.OperationName) == AwsOperation
             && base.ShouldInspect(span);
        }

        public override string Detail()
        {
            return base.Detail() + $" aws.operation={AwsOperation}";
        }
    }
}
