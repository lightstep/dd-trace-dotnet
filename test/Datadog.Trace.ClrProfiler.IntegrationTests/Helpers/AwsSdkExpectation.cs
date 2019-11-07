using System.Collections.Generic;
using Datadog.Trace.TestHelpers;

namespace Datadog.Trace.ClrProfiler.IntegrationTests
{
    public class AwsSdkExpectation : SpanExpectation
    {
        public const string IntegrationOperationName = "aws.http";

        public AwsSdkExpectation(string serviceName)
        : base(serviceName, IntegrationOperationName, SpanTypes.Http)
        {
            RegisterTagExpectation(Tags.SpanKind, expected: SpanKinds.Client, when: Always);
        }

        public class Commands
        {
            public const string CreateQueueRequest = "CreateQueue";
            public const string ListQueuesRequest = "ListQueues";
            public const string GetQueueUrlRequest = "GetQueueUrl";
            public const string SendMessageRequest = "SendMessage";
            public const string DeleteMessageRequest = "DeleteMessage";
            public const string SendMessageBatchRequest = "SendMessageBatch";
            public const string ReceiveMessageRequest = "ReceiveMessage";
            public const string DeleteMessageBatchRequest = "DeleteMessageBatch";
            public const string PurgeQueueRequest = "PurgeQueue";
            public const string DeleteQueueRequest = "DeleteQueue";
        }
    }
}
