using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using msmq = System.Messaging;

namespace Sixeyed.MessageQueue.Messaging.Msmq
{
    public class MsmqMessageQueue : MessageQueueBase
    {
        private msmq.MessageQueue _queue;

        public override void InitialiseInbound(string name, MessagePattern pattern, bool isTemporary,
                                               Dictionary<string, object> properties = null)
        {
            Initialise(Direction.Inbound, name, pattern, isTemporary, properties);
            _queue = new msmq.MessageQueue(Address);
        }

        public override void InitialiseOutbound(string name, MessagePattern pattern, bool isTemporary, 
                                                Dictionary<string, object> properties = null)
        {
            Initialise(Direction.Outbound, name, pattern, isTemporary, properties);
            _queue = new msmq.MessageQueue(Address);
        }

        public override IMessageQueue GetResponseQueue()
        {
            if (!(Pattern == MessagePattern.RequestResponse && Direction == Direction.Outbound))
                throw new InvalidOperationException("Cannot get a response queue except for outbound request-response");

            var responseAddress = string.Format(".\\private$\\{0}", Guid.NewGuid().ToString().Substring(0, 6));
            msmq.MessageQueue.Create(responseAddress);

            var responseQueue = MessageQueueFactory.CreateInbound(responseAddress, MessagePattern.RequestResponse, true);
            return responseQueue;
        }

        public override IMessageQueue GetReplyQueue(Message message)
        {
            if (!(Pattern == MessagePattern.RequestResponse && Direction == Direction.Inbound))
                throw new InvalidOperationException("Cannot get a reply queue except for inbound request-response");

            var responseQueue = MessageQueueFactory.CreateOutbound(message.ResponseAddress, MessagePattern.RequestResponse, true);
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

        public override void Receive(Action<Message> onMessageReceived, bool processAsync, int maximumWaitMilliseconds = 0)
        {
            msmq.Message inbound;
            if (maximumWaitMilliseconds > 0)
            {
                inbound = _queue.Receive(TimeSpan.FromMilliseconds(maximumWaitMilliseconds));
            }
            else
            {
                inbound = _queue.Receive();
            }
            var message = Message.FromJson(inbound.BodyStream);
            if (processAsync)
            {
                Task.Factory.StartNew(() => onMessageReceived(message));
            }
            else
            {
                onMessageReceived(message);
            }
        }

        public override string GetAddress(string name)
        {
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

        public override void DeleteQueue()
        {
            if (msmq.MessageQueue.Exists(Address))
            {
                msmq.MessageQueue.Delete(Address);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _queue != null)
            {
                _queue.Dispose();                
            }
        }
    }
}
