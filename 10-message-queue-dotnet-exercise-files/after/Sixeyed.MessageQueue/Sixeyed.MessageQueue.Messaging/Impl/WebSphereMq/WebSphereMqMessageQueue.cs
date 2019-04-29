using IBM.WMQ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixeyed.MessageQueue.Messaging.Impl.WebSphereMq
{
    public class WebSphereMqMessageQueue : MessageQueueBase
    {
        private MQQueueManager _queueManager;
        private MQQueue _queue;
        private MQTopic _topic;

        public override string Name
        {
            get { return "WebSphereMQ"; }
        }

        protected override string CreateResponseQueue()
        {
            var queueManagerName = RequirePropertyValue("queuemanager");
            var responseQueueName = "";
            using (var responseQueue = _queueManager.AccessQueue("dynamic.response.model", 
                MQC.MQOO_INPUT_SHARED, queueManagerName, "response.*", ""))
            {
                responseQueueName = responseQueue.Name;
            }
            _queueManager.Commit();
            return responseQueueName;
        }

        public override void InitialiseOutbound(string name, MessagePattern pattern, bool isTemporary)
        {
            Initialise(Direction.Outbound, name, pattern, isTemporary);
            var queueManagerName = InitialiseQueueManager();
            if (Pattern == MessagePattern.PublishSubscribe)
            {
                _topic = _queueManager.AccessTopic(Address, "", 
                    MQC.MQTOPIC_OPEN_AS_PUBLICATION, MQC.MQOO_OUTPUT);
            }
            else if (Pattern == MessagePattern.RequestResponse && IsTemporary)
            {
                _queue = _queueManager.AccessQueue(Address, MQC.MQOO_OUTPUT, 
                    queueManagerName, null, null);
            }
            else
            {
                _queue = _queueManager.AccessQueue(Address, MQC.MQOO_OUTPUT);
            }
        }

        private string InitialiseQueueManager()
        {
            var queueManagerName = RequirePropertyValue("queuemanager");
            var properties = new Hashtable();
            foreach(var property in Properties.Where(x=>x.Key != "queuemanager"))
            {
                properties.Add(property.Key, property.Value);
            }
            _queueManager = new MQQueueManager(queueManagerName, properties);
            return queueManagerName;
        }

        public override void InitialiseInbound(string name, MessagePattern pattern, bool isTemporary)
        {
            Initialise(Direction.Inbound, name, pattern, isTemporary);
            var queueManagerName = InitialiseQueueManager();

            if (Pattern == MessagePattern.RequestResponse && isTemporary)
            {
                _queue = _queueManager.AccessQueue("dynamic.response.model", 
                    MQC.MQOO_INPUT_SHARED, queueManagerName, Address, "");
            }
            else
            {
                _queue = _queueManager.AccessQueue(Address, MQC.MQOO_INPUT_AS_Q_DEF);
            }
        }

        public override void Send(Message message)
        {
            var outgoing = new MQMessage();
            outgoing.Format = MQC.MQFMT_STRING;
            outgoing.WriteString(message.ToJsonString());
            if (Pattern == MessagePattern.PublishSubscribe)
            {
                _topic.Put(outgoing);
            }
            else
            {
                _queue.Put(outgoing);
            }
            _queueManager.Commit();
        }

        public override void DeleteQueue()
        {
            //do nothing
        }

        public override void Receive(Action<Message> onMessageReceived, bool processAsync, int maximumWaitMilliseconds = 0)
        {
            try
            {
                var inbound = new MQMessage
                {
                    Format = MQC.MQFMT_STRING
                };
                if (maximumWaitMilliseconds > 0)
                {
                    var options = new MQGetMessageOptions
                    {
                        WaitInterval = maximumWaitMilliseconds,
                        Options = MQC.MQGMO_WAIT
                    };
                    _queue.Get(inbound, options);
                }
                else
                {
                    _queue.Get(inbound);
                }
                var messageJson = inbound.ReadString(inbound.MessageLength);
                var message = Message.FromJson(messageJson);
                if (processAsync)
                {
                    Task.Factory.StartNew(() => onMessageReceived(message));
                }
                else
                {
                    onMessageReceived(message);
                }
                _queueManager.Commit();
            }
            catch(MQException mqEx)
            {
                if (mqEx.Reason != MQC.MQRC_NO_MSG_AVAILABLE)
                    throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_queueManager != null)
                {
                    ((IDisposable) _queueManager).Dispose();
                }
                if (_queue != null)
                {
                    ((IDisposable)_queue).Dispose();
                }
            }
        }
    }
}
