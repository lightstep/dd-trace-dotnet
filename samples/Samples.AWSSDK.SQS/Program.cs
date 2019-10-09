
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Samples.AWSSDK.SQS
{
    public class Program
    {
        static IAmazonSQS sqsClient;
        static string queueUrl;
        const string QueueName = "MySQSQueue";

        // See https://markmcgookin.com/2017/03/17/posting-to-amazon-sqs-with-net-core/
        static async Task Main(string[] args)
        {
            // Or use an AmazonSQSConfig
            // source: https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/InitSQSClient.html
            var awsCreds = new BasicAWSCredentials("x", "x");
            var sqsConfig = new AmazonSQSConfig();
            sqsConfig.ServiceURL = "http://localhost:9324";
            sqsClient = new AmazonSQSClient(awsCreds, sqsConfig);

            // Or set up SQS client from other documentation
            // source: https://markmcgookin.com/2017/03/17/posting-to-amazon-sqs-with-net-core/
            // var awsCreds = new BasicAWSCredentials("x", "x");
            // sqsClient = new AmazonSQSClient(awsCreds, Amazon.RegionEndpoint.EUWest1);

#if NETFRAMEWORK
            CreateSqsQueue();
            ListQueues();
            queueUrl = GetQueueUrl();
            SendMessage();
            SendMessageBatch();
            ReceiveMessages();
            DeleteQueue();
#endif

            await CreateSqsQueueAsync();
            await ListQueuesAsync();
            queueUrl = await GetQueueUrlAsync();
            await SendMessageAsync();
            await SendMessageBatchAsync();
            await ReceiveMessagesAsync();
            await DeleteQueueAsync();
        }

#if NETFRAMEWORK
        // Create queue
        // https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/CreateQueue.html
        static void CreateSqsQueue()
        {
            var createQueueRequest = new CreateQueueRequest();

            createQueueRequest.QueueName = QueueName;
            var attrs = new Dictionary<string, string>();
            attrs.Add(QueueAttributeName.VisibilityTimeout, "0");
            createQueueRequest.Attributes = attrs;
            var createQueueResponse = sqsClient.CreateQueue(createQueueRequest);
        }

        // List queues
        // source: https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/UsingSQSQueues.html
        static void ListQueues()
        {
            var listQueuesResponse = sqsClient.ListQueues(new ListQueuesRequest());
            foreach (var responseQueueUrl in listQueuesResponse.QueueUrls)
            {
                Console.WriteLine(responseQueueUrl);
            }
        }

        // Get queue url
        // source: https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/UsingSQSQueues.html
        static string GetQueueUrl()
        {
            var getQueueUrlRequest = new GetQueueUrlRequest
            {
                QueueName = QueueName
            };

            var getQueueUrlResponse = sqsClient.GetQueueUrl(getQueueUrlRequest);
            Console.WriteLine("The SQS queue's URL is {0}", getQueueUrlResponse.QueueUrl);

            // Set queue url for later calls
            return getQueueUrlResponse.QueueUrl;
        }

        // Send a message to the queue
        // source: https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/SendMessage.html
        static void SendMessage()
        {
            var sendRequest = new SendMessageRequest();
            sendRequest.QueueUrl = queueUrl;
            sendRequest.MessageBody = "{ 'message' : 'hello world' }";

            var sendMessageResponse = sqsClient.SendMessage(sendRequest);
        }

        // Send a batch of messages to the queue
        // source: https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/SendMessageBatch.html
        static void SendMessageBatch()
        {
            var sendMessageBatchRequest = new SendMessageBatchRequest
            {
                Entries = new List<SendMessageBatchRequestEntry>
                {
                    new SendMessageBatchRequestEntry("message1", "FirstMessageContent"),
                    new SendMessageBatchRequestEntry("message2", "SecondMessageContent"),
                    new SendMessageBatchRequestEntry("message3", "ThirdMessageContent")
                },
                QueueUrl = queueUrl
            };

            var sendMessageBatchResponse = sqsClient.SendMessageBatch(sendMessageBatchRequest);
            Console.WriteLine("Messages successfully sent:");
            foreach (var success in sendMessageBatchResponse.Successful)
            {
                Console.WriteLine("    Message id : {0}", success.MessageId);
                Console.WriteLine("    Message content MD5 : {0}", success.MD5OfMessageBody);
            }

            Console.WriteLine("Messages failed to send:");
            foreach (var failed in sendMessageBatchResponse.Failed)
            {
                Console.WriteLine("    Message id : {0}", failed.Id);
                Console.WriteLine("    Message content : {0}", failed.Message);
                Console.WriteLine("    Sender's fault? : {0}", failed.SenderFault);
            }
        }

        // Receive messages from the queue
        // source: https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/ReceiveMessage.html
        static void ReceiveMessages()
        {
            var receiveMessageRequest = new ReceiveMessageRequest();
            receiveMessageRequest.QueueUrl = queueUrl;

            var receiveMessageResponse = sqsClient.ReceiveMessage(receiveMessageRequest);
            foreach (var message in receiveMessageResponse.Messages)
            {
                Console.WriteLine(message.Body);

                // Delete the message from the queue
                // source: https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/DeleteMessage.html
                var deleteMessageRequest = new DeleteMessageRequest();
                deleteMessageRequest.QueueUrl = queueUrl;
                deleteMessageRequest.ReceiptHandle = message.ReceiptHandle;

                var deleteMessageResponse = sqsClient.DeleteMessage(deleteMessageRequest);
            }
        }

        // Delete queue
        // source: https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/UsingSQSQueues.html
        static void DeleteQueue()
        {
            var deleteQueueRequest = new DeleteQueueRequest
            {
                QueueUrl = queueUrl
            };
            sqsClient.DeleteQueue(deleteQueueRequest);
        }

#endif
        // Create queue
        // https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/CreateQueue.html
        static async Task CreateSqsQueueAsync()
        {
            var createQueueRequest = new CreateQueueRequest();

            createQueueRequest.QueueName = QueueName;
            var attrs = new Dictionary<string, string>();
            attrs.Add(QueueAttributeName.VisibilityTimeout, "0");
            createQueueRequest.Attributes = attrs;
            var createQueueResponse = await sqsClient.CreateQueueAsync(createQueueRequest);
        }

        // List queues
        // source: https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/UsingSQSQueues.html
        static async Task ListQueuesAsync()
        {
            var listQueuesResponse = await sqsClient.ListQueuesAsync(new ListQueuesRequest());
            foreach (var responseQueueUrl in listQueuesResponse.QueueUrls)
            {
                Console.WriteLine(responseQueueUrl);
            }
        }

        // Get queue url
        // source: https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/UsingSQSQueues.html
        static async Task<string> GetQueueUrlAsync()
        {
            var getQueueUrlRequest = new GetQueueUrlRequest
            {
                QueueName = QueueName
            };

            var getQueueUrlResponse = await sqsClient.GetQueueUrlAsync(getQueueUrlRequest);
            Console.WriteLine("The SQS queue's URL is {0}", getQueueUrlResponse.QueueUrl);

            // Set queue url for later calls
            return getQueueUrlResponse.QueueUrl;
        }

        // Send a message to the queue
        // source: https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/SendMessage.html
        static async Task SendMessageAsync()
        {
            var sendRequest = new SendMessageRequest();
            sendRequest.QueueUrl = queueUrl;
            sendRequest.MessageBody = "{ 'message' : 'hello world' }";

            var sendMessageResponse = await sqsClient.SendMessageAsync(sendRequest);
        }

        // Send a batch of messages to the queue
        // source: https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/SendMessageBatch.html
        static async Task SendMessageBatchAsync()
        {
            var sendMessageBatchRequest = new SendMessageBatchRequest
            {
                Entries = new List<SendMessageBatchRequestEntry>
                {
                    new SendMessageBatchRequestEntry("message1", "FirstMessageContent"),
                    new SendMessageBatchRequestEntry("message2", "SecondMessageContent"),
                    new SendMessageBatchRequestEntry("message3", "ThirdMessageContent")
                },
                QueueUrl = queueUrl
            };

            var sendMessageBatchResponse = await sqsClient.SendMessageBatchAsync(sendMessageBatchRequest);
            Console.WriteLine("Messages successfully sent:");
            foreach (var success in sendMessageBatchResponse.Successful)
            {
                Console.WriteLine("    Message id : {0}", success.MessageId);
                Console.WriteLine("    Message content MD5 : {0}", success.MD5OfMessageBody);
            }

            Console.WriteLine("Messages failed to send:");
            foreach (var failed in sendMessageBatchResponse.Failed)
            {
                Console.WriteLine("    Message id : {0}", failed.Id);
                Console.WriteLine("    Message content : {0}", failed.Message);
                Console.WriteLine("    Sender's fault? : {0}", failed.SenderFault);
            }
        }

        // Receive messages from the queue
        // source: https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/ReceiveMessage.html
        static async Task ReceiveMessagesAsync()
        {
            var receiveMessageRequest = new ReceiveMessageRequest();
            receiveMessageRequest.QueueUrl = queueUrl;

            var receiveMessageResponse = await sqsClient.ReceiveMessageAsync(receiveMessageRequest);
            foreach (var message in receiveMessageResponse.Messages)
            {
                Console.WriteLine(message.Body);

                // Delete the message from the queue
                // source: https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/DeleteMessage.html
                var deleteMessageRequest = new DeleteMessageRequest();
                deleteMessageRequest.QueueUrl = queueUrl;
                deleteMessageRequest.ReceiptHandle = message.ReceiptHandle;

                var deleteMessageResponse = await sqsClient.DeleteMessageAsync(deleteMessageRequest);
            }
        }

        // Delete queue
        // source: https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/UsingSQSQueues.html
        static async Task DeleteQueueAsync()
        {
            var deleteQueueRequest = new DeleteQueueRequest
            {
                QueueUrl = queueUrl
            };
            await sqsClient.DeleteQueueAsync(deleteQueueRequest);
        }
    }
}
