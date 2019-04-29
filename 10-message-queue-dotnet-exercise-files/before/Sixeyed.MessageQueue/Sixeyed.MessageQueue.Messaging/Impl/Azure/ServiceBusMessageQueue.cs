using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Sixeyed.MessageQueue.Messaging.Impl.Azure
{
    public class ServiceBusMessageQueue : MessageQueueBase
    {
        private QueueClient _queueClient;
        private TopicClient _topicClient;
        private SubscriptionClient _subscriptionClient;
        private Action<Message> _handleMessage;

        public override void InitialiseOutbound(string name, MessagePattern pattern, bool isTemporary)
        {
            Initialise(Direction.Outbound, name, pattern, isTemporary);
            var factory = MessagingFactory.CreateFromConnectionString(GetConnectionString());
            if (Pattern == MessagePattern.PublishSubscribe)
            {
                _topicClient = factory.CreateTopicClient(Address);
            }
            else
            {
                _queueClient = factory.CreateQueueClient(Address);
            }
        }

        public override void InitialiseInbound(string name, MessagePattern pattern, bool isTemporary)
        {
            Initialise(Direction.Inbound, name, pattern, isTemporary);
            var factory = MessagingFactory.CreateFromConnectionString(GetConnectionString());
            if (Pattern == MessagePattern.PublishSubscribe)
            {
                var addressParts = Address.Split(':');
                _subscriptionClient = factory.CreateSubscriptionClient(addressParts[0], addressParts[1]);
            }
            else
            {
                _queueClient = factory.CreateQueueClient(Address);
            }
        }

        public override void Send(Message message)
        {
            var brokeredMessage = new BrokeredMessage(message.ToJsonStream(), true);
            if (Pattern == MessagePattern.PublishSubscribe)
            {
                _topicClient.Send(brokeredMessage);
            }
            else
            {
                _queueClient.Send(brokeredMessage);
            }
        }

        protected override void ListenInternal(Action<Message> onMessageReceived, CancellationToken cancellationToken)
        {
            _handleMessage = onMessageReceived;
            var options = new OnMessageOptions
                {
                    MaxConcurrentCalls = 10
                };
            if (Pattern == MessagePattern.PublishSubscribe)
            {
                _subscriptionClient.OnMessage(Handle, options);
            }
            else
            {
                _queueClient.OnMessage(Handle, options);
            }
            cancellationToken.WaitHandle.WaitOne();
        }

        public override void Receive(Action<Message> onMessageReceived, bool processAsync,
                                     int maximumWaitMilliseconds = 0)
        {
            _handleMessage = onMessageReceived;
            BrokeredMessage brokeredMessage;
            if (Pattern == MessagePattern.PublishSubscribe)
            {
                brokeredMessage = _subscriptionClient.Receive(TimeSpan.FromMilliseconds(maximumWaitMilliseconds));
            }
            else
            {
                brokeredMessage = _queueClient.Receive(TimeSpan.FromMilliseconds(maximumWaitMilliseconds));
            }
            if (processAsync)
            {
                Task.Factory.StartNew(() => Handle(brokeredMessage));
            }
            else
            {
                Handle(brokeredMessage);
            }
        }

        private void Handle(BrokeredMessage brokeredMessage)
        {
            var messageStream = brokeredMessage.GetBody<Stream>();
            var message = Message.FromJson(messageStream);
            _handleMessage(message);
            brokeredMessage.Complete();
        }

        public override IMessageQueue GetReplyQueue(Message message)
        {
            var replyQueue = MessageQueueFactory.CreateOutbound(message.ResponseAddress, MessagePattern.RequestResponse,
                                                                true);
            return replyQueue;
        }

        public override void DeleteQueue()
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(GetConnectionString());
            if (namespaceManager.QueueExists(Address))
            {
                namespaceManager.DeleteQueue(Address);
            }
        }

        protected override void Dispose(bool disposing)
        {
            //
        }

        public override string Name
        {
            get { return "Azure"; }
        }

        protected override string CreateResponseQueue()
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(GetConnectionString());
            var responseAddress = Guid.NewGuid().ToString().Substring(0, 6);
            namespaceManager.CreateQueue(responseAddress);
            return responseAddress;
        }

        private string GetConnectionString()
        {
            return RequirePropertyValue("connectionstring");
        }
    }
}