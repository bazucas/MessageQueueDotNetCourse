using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using msmq = System.Messaging;

namespace Sixeyed.MessageQueue.Messaging.Msmq
{
    public class MsmqMessageQueue : MessageQueueBase
    {
        private msmq.MessageQueue _queue;

        public override string Name
        {
            get { return "MSMQ"; }
        }

        public override void InitialiseInbound(string name, MessagePattern pattern, bool isTemporary)
        {
            Initialise(Direction.Inbound, name, pattern, isTemporary);
            _queue = new msmq.MessageQueue(Address);
            if (Pattern == MessagePattern.PublishSubscribe)
            {
                _queue.MulticastAddress = RequirePropertyValue("MulticastAddress");
            }
        }

        public override void InitialiseOutbound(string name, MessagePattern pattern, bool isTemporary)
        {
            Initialise(Direction.Outbound, name, pattern, isTemporary);
            _queue = new msmq.MessageQueue(Address);
        }

        protected override string CreateResponseQueue()
        {
            var address = string.Format(".\\private$\\{0}", Guid.NewGuid().ToString().Substring(0, 6));
            msmq.MessageQueue.Create(address);
            return address;
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

        public override void DeleteQueue()
        {
            if (!IsTemporary)
            {
                throw new InvalidOperationException("Only temporary queues can be deleted");
            }
            msmq.MessageQueue.Delete(Address);
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
