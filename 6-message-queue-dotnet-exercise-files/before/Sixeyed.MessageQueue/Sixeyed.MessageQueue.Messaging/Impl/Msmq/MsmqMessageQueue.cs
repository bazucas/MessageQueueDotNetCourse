using System;
using System.Collections.Generic;
using System.Linq;
using msmq = System.Messaging;

namespace Sixeyed.MessageQueue.Messaging.Msmq
{
    public class MsmqMessageQueue : MessageQueueBase
    {
        private msmq.MessageQueue _queue;
        private bool _useTemporaryQueue;

        public override void InitialiseInbound(string name, MessagePattern pattern,
                                               Dictionary<string, object> properties = null)
        {
            Initialise(Direction.Inbound, name, pattern, properties);
            switch (Pattern)
            {
                case MessagePattern.PublishSubscribe:
                    //RequireProperty<string>("MulticastAddress");
                    _queue = new msmq.MessageQueue(Address);
                    //_queue.MulticastAddress = GetPropertyValue<string>("MulticastAddress");
                    break;

                case MessagePattern.RequestResponse:
                    _queue = _useTemporaryQueue ? msmq.MessageQueue.Create(Address) : new msmq.MessageQueue(Address);
                    break;

                default:
                    _queue = new msmq.MessageQueue(Address);
                    break;
            }
        }

        public override void InitialiseOutbound(string name, MessagePattern pattern, Dictionary<string, object> properties = null)
        {
            Initialise(Direction.Outbound, name, pattern, properties);
            _queue = new msmq.MessageQueue(Address);
        }

        public override IMessageQueue GetResponseQueue()
        {
            if (!(Pattern == MessagePattern.RequestResponse && Direction == Direction.Outbound))
                throw new InvalidOperationException("Cannot get a response queue except for outbound request-response");

            var responseQueue = new MsmqMessageQueue();
            responseQueue.InitialiseInbound(null, MessagePattern.RequestResponse);
            return responseQueue;
        }

        public override IMessageQueue GetReplyQueue(Message message)
        {
            if (!(Pattern == MessagePattern.RequestResponse && Direction == Direction.Inbound))
                throw new InvalidOperationException("Cannot get a reply queue except for inbound request-response");

            var responseQueue = new MsmqMessageQueue();
            responseQueue.InitialiseOutbound(message.ResponseAddress, MessagePattern.RequestResponse);
            return responseQueue;
        }

        public override void Send(Message message)
        {
            var outbound = new msmq.Message();
            outbound.BodyStream = message.ToJsonStream();
            if (!string.IsNullOrEmpty(message.ResponseAddress))
            {
                outbound.ResponseQueue = new msmq.MessageQueue(message.ResponseAddress);
            }
            _queue.Send(outbound);
        }

        public override void Listen(Action<Message> onMessageReceived)
        {
            while (true)
            {
                Receive(onMessageReceived);
            }
        }

        public override void Receive(Action<Message> onMessageReceived)
        {
            var inbound = _queue.Receive();
            var message = Message.FromJson(inbound.BodyStream);
            onMessageReceived(message);
        }

        public override string GetAddress(string name)
        {
            if (Pattern == MessagePattern.RequestResponse && Direction == Direction.Inbound && string.IsNullOrEmpty(name))
            {
                _useTemporaryQueue = true;
                return string.Format(".\\private$\\{0}", Guid.NewGuid().ToString().Substring(0, 6));
            }

            switch (name.ToLower())
            {
                case "unsubscribe":
                    return ".\\private$\\sixeyed.messagequeue.unsubscribe";

                case "doesuserexist":
                    return ".\\private$\\sixeyed.messagequeue.doesuserexist";

                case "unsubscribed-event":
                    return  "FormatName:MULTICAST=234.1.1.2:8001";

                case "unsubscribe-legacy":
                    return ".\\private$\\sixeyed.messagequeue.unsubscribe-legacy";

                case "unsubscribe-crm":
                    return ".\\private$\\sixeyed.messagequeue.unsubscribe-crm";

                case "unsubscribe-fulfilment":
                    return ".\\private$\\sixeyed.messagequeue.unsubscribe-fulfilment";

                default:
                    return name;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _queue != null)
            {
                _queue.Dispose();
                if (_useTemporaryQueue && msmq.MessageQueue.Exists(Address))
                {
                    msmq.MessageQueue.Delete(Address);
                }
            }
        }
    }
}
