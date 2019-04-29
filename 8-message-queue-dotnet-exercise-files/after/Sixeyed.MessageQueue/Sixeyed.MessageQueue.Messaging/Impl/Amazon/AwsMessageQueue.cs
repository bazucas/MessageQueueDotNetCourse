using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using sqs = Amazon.SQS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixeyed.MessageQueue.Messaging.Impl.Amazon
{
    public class AwsMessageQueue : MessageQueueBase
    {
        private AmazonSQSClient _sqsClient;
        private AmazonSimpleNotificationServiceClient _snsClient;
        private const string _accessKey = "YOUR-ACCESS-KEY";
        private const string _secretKey = "YOUR-SECRET-KEY";

        public override void InitialiseOutbound(string name, MessagePattern pattern, bool isTemporary, Dictionary<string, object> properties = null)
        {
            Initialise(Direction.Outbound, name, pattern, isTemporary, properties);
            if (Pattern == MessagePattern.PublishSubscribe)
            {
                _snsClient = new AmazonSimpleNotificationServiceClient(_accessKey, _secretKey, RegionEndpoint.EUWest1);
            }
            else
            {
                _sqsClient = new AmazonSQSClient(_accessKey, _secretKey, RegionEndpoint.EUWest1);
            }
        }

        public override void InitialiseInbound(string name, MessagePattern pattern, bool isTemporary, Dictionary<string, object> properties = null)
        {
            Initialise(Direction.Inbound, name, pattern, isTemporary, properties);
            _sqsClient = new AmazonSQSClient(_accessKey, _secretKey, RegionEndpoint.EUWest1);        
        }

        public override void Send(Message message)
        {
            if (Pattern == MessagePattern.PublishSubscribe)
            {
                var publishRequest = new PublishRequest();
                publishRequest.Message = message.ToJsonString();
                publishRequest.TopicArn = Address;
                _snsClient.Publish(publishRequest);
            }
            else
            {
                var sendRequest = new SendMessageRequest();
                sendRequest.MessageBody = message.ToJsonString();
                sendRequest.QueueUrl = Address;
                _sqsClient.SendMessage(sendRequest);
            }
        }

        public override void Receive(Action<Message> onMessageReceived, bool processAsync, int maximumWaitMilliseconds = 0)
        {
            var receiveRequest = new ReceiveMessageRequest();
            receiveRequest.QueueUrl = Address;
            receiveRequest.WaitTimeSeconds = maximumWaitMilliseconds / 1000;
            receiveRequest.MaxNumberOfMessages = 10;
            var response = _sqsClient.ReceiveMessage(receiveRequest);
            if (response.Messages.Any())
            {
                response.Messages.Where(x => x != null).ToList().ForEach(x =>
                    {
                        if (processAsync)
                        {
                            Task.Factory.StartNew(() => Handle(x, onMessageReceived));
                        }
                        else
                        {
                            Handle(x, onMessageReceived);
                        }
                    });
            }
        }

        private void Handle(sqs.Message sqsMessage, Action<Message> onMessageReceived)
        {
            var message = Message.FromJson(sqsMessage.Body);
            onMessageReceived(message);

            var deleteRequest = new DeleteMessageRequest();
            deleteRequest.QueueUrl = Address;
            deleteRequest.ReceiptHandle = sqsMessage.ReceiptHandle;
            _sqsClient.DeleteMessage(deleteRequest);
        }

        public override string GetAddress(string name)
        {
            switch(name.ToLower())
            {
                case "doesuserexist":
                    return "https://sqs.eu-west-1.amazonaws.com/063992587608/doesuserexist";

                case "unsubscribe":
                    return "https://sqs.eu-west-1.amazonaws.com/063992587608/unsubscribe";

                case "unsubscribed-event":
                    return "arn:aws:sns:eu-west-1:063992587608:unsubscribed-event";

                case "unsubscribe-crm":
                    return "https://sqs.eu-west-1.amazonaws.com/063992587608/unsubscribe-crm";

                case "unsubscribe-fulfilment":
                    return "https://sqs.eu-west-1.amazonaws.com/063992587608/unsubscribe-fulfilment";

                case "unsubscribe-legacy":
                    return "https://sqs.eu-west-1.amazonaws.com/063992587608/unsubscribe-legacy";

                default:
                    return name;
            }
        }

        public override IMessageQueue GetResponseQueue()
        {
            var createRequest = new CreateQueueRequest();
            createRequest.QueueName = Guid.NewGuid().ToString().Substring(0, 6);
            var createResponse = _sqsClient.CreateQueue(createRequest);

            var responseQueue = MessageQueueFactory.CreateInbound(createResponse.QueueUrl, MessagePattern.RequestResponse, true);
            return responseQueue;
        }

        public override IMessageQueue GetReplyQueue(Message message)
        {
            var replyQueue = MessageQueueFactory.CreateOutbound(message.ResponseAddress, MessagePattern.RequestResponse, true);
            return replyQueue;
        }

        public override void DeleteQueue()
        {
            var deleteQueueRequest = new DeleteQueueRequest();
            deleteQueueRequest.QueueUrl = Address;
            _sqsClient.DeleteQueue(deleteQueueRequest);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _sqsClient != null)
            {
                _sqsClient.Dispose();
            }
        }
    }
}
