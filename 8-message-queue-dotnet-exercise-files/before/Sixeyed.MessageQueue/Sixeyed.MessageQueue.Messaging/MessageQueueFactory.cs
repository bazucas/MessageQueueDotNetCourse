using Sixeyed.MessageQueue.Messaging.Impl.ZeroMq;
using Sixeyed.MessageQueue.Messaging.Msmq;
using System.Collections.Generic;

namespace Sixeyed.MessageQueue.Messaging
{
    public static class MessageQueueFactory
    {
        private static Dictionary<string, IMessageQueue> _Queues = new Dictionary<string, IMessageQueue>();

        public static IMessageQueue CreateInbound(string name, MessagePattern pattern,
                                                  Dictionary<string, object> properties = null)
        {
            var key = string.Format("{0}:{1}:{2}", Direction.Inbound, name, pattern);
            if (_Queues.ContainsKey(key))
                return _Queues[key];

            var queue = Create();
            queue.InitialiseInbound(name, pattern, properties);
            _Queues[key] = queue;
            return _Queues[key];
        }

        public static IMessageQueue CreateOutbound(string name, MessagePattern pattern,
                                                   Dictionary<string, object> properties = null)
        {
            var key = string.Format("{0}:{1}:{2}", Direction.Outbound, name, pattern);
            if (_Queues.ContainsKey(key))
                return _Queues[key];

            var queue = Create();
            queue.InitialiseOutbound(name, pattern, properties);
            _Queues[key] = queue;
            return _Queues[key];
        }

        private static IMessageQueue Create()
        {
            return new MsmqMessageQueue();
            //return new ZeroMqMessageQueue();
        }
    }
}