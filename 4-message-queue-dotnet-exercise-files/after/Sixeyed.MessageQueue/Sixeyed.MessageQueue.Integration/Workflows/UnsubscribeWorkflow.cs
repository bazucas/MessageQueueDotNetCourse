using Sixeyed.MessageQueue.Integration.Data;
using Sixeyed.MessageQueue.Messages;
using Sixeyed.MessageQueue.Messages.Event;
using System;
using System.Linq;
using msmq = System.Messaging;

namespace Sixeyed.MessageQueue.Integration.Workflows
{
    public  class UnsubscribeWorkflow
    {
        public const int StepDuration = 250; //10000; 

        public string EmailAddress { get; private set; }

        public UnsubscribeWorkflow(string emailAddress)
        {
            EmailAddress = emailAddress;
        }

        public void Run()
        {
            PersistAsUnsubscribed();
            NotifyUserUnsubscribed();
        }

        private void NotifyUserUnsubscribed()
        {
            var unsubscribedEvent = new UserUnsubscribed
            {
                EmailAddress = EmailAddress
            };
            using (var queue = new msmq.MessageQueue("FormatName:MULTICAST=234.1.1.2:8001"))
            {
                var message = new msmq.Message();
                message.BodyStream = unsubscribedEvent.ToJsonStream();
                message.Label = unsubscribedEvent.GetMessageType();
                message.Recoverable = true;
                queue.Send(message);
            }
        }

        private void PersistAsUnsubscribed()
        {
            using (var context = new UserModelContainer())
            {
                var user = context.Users.Single(x => x.EmailAddress == EmailAddress);
                user.IsUnsubscribed = true;
                user.UnsubscribedAt = DateTime.Now;
                context.SaveChanges();
            }
        }
    }
}
