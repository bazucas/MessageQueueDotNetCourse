using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZMQ;

namespace Sixeyed.MessageQueue.Messaging.ZeroMq
{
    public class ZeroMqMessageQueue : MessageQueueBase
    {
        private static Context _Context;
        private static object _ContextLock = new object();
        private Socket _socket;

        public override string Name
        {
            get { return "ZeroMQ"; }
        }

        public override void InitialiseOutbound(string name, MessagePattern pattern, bool isTemporary)
        {
            EnsureContext();
            Initialise(Direction.Outbound, name, pattern, isTemporary);
            switch (Pattern)
            {
                case MessagePattern.PublishSubscribe:
                    _socket = _Context.Socket(SocketType.PUB);
                    _socket.Bind(Address);
                    break;

                case MessagePattern.RequestResponse:
                    _socket = _Context.Socket(SocketType.REQ);
                    _socket.Connect(Address);
                    break;

                case MessagePattern.FireAndForget:
                    _socket = _Context.Socket(SocketType.PUSH);
                    _socket.Connect(Address);
                    break;
            }
        }

        public override void InitialiseInbound(string name, MessagePattern pattern, bool isTemporary)
        {
            EnsureContext();
            Initialise(Direction.Inbound, name, pattern, isTemporary);
            switch (Pattern)
            {
                case MessagePattern.PublishSubscribe:
                    _socket = _Context.Socket(SocketType.SUB);
                    _socket.Connect(Address);
                    _socket.Subscribe("", Encoding.UTF8);
                    break;

                case MessagePattern.RequestResponse:
                    _socket = _Context.Socket(SocketType.REP);
                    _socket.Bind(Address);
                    break;

                case MessagePattern.FireAndForget:
                    _socket = _Context.Socket(SocketType.PULL);
                    _socket.Bind(Address);
                    break;
            }
        }

        public override IMessageQueue GetResponseQueue()
        {
            if (!(Pattern == MessagePattern.RequestResponse && Direction == Direction.Outbound))
                throw new InvalidOperationException("Cannot get a response queue except for outbound request-response");

            return this;
        }

        public override IMessageQueue GetReplyQueue(Message message)
        {
            if (!(Pattern == MessagePattern.RequestResponse && Direction == Direction.Inbound))
                throw new InvalidOperationException("Cannot get a reply queue except for inbound request-response");

            return this;
        }

        public override void Send(Message message)
        {
            var messageJson = message.ToJsonString();
            _socket.Send(messageJson, Encoding.UTF8);
        }

        public override void Receive(Action<Message> onMessageReceived, bool processAsync, int maximumWaitMilliseconds = 0)
        {
            string inbound;
            if (maximumWaitMilliseconds > 0)
            {
                inbound = _socket.Recv(Encoding.UTF8, maximumWaitMilliseconds);
            }
            else
            {
                inbound = _socket.Recv(Encoding.UTF8);
            }
            var message = Message.FromJson(inbound);
            //we can only process ZMQ async if the pattern supports it - we can't call Rec
            //twice on a REP socket without the Send in between:
            if (processAsync && Pattern != MessagePattern.RequestResponse)
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
            //do nothing - there is no queue
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing && _socket != null)
            {
                _socket.Dispose();
            }
        }

        private static void EnsureContext()
        {
            if (_Context == null)
            {
                lock (_ContextLock)
                {
                    if (_Context == null)
                    {
                        _Context = new Context();
                    }
                }
            }
        }

        protected override string CreateResponseQueue()
        {
            throw new InvalidOperationException("Cannot create a response queue for ZeroMQ");
        }
    }
}