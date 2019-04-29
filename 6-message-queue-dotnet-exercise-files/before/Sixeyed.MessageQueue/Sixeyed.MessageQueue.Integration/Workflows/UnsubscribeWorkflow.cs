using Sixeyed.MessageQueue.Integration.Data;
using Sixeyed.MessageQueue.Messages.Events;
using Sixeyed.MessageQueue.Messaging;
using System;
using System.Linq;

namespace Sixeyed.MessageQueue.Integration.Workflows
{
    public class UnsubscribeWorkflow
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
            SendNotificationEvent();
        }

        private void PersistAsUnsubscribed()
        {
            using (var context = new UserModelContainer())
            {
                var user = context.Users.Single(x => x.EmailAddress == EmailAddress);
                user.IsUnsubscribed = true;
                user.UnsubscribedAt = DateTime.Now;
                user.UserEvents.Add(new UserEvent
                    {
                        EventCode = "UNSUB",
                        RecordedAt = DateTime.Now
                    });
                context.SaveChanges();
            }
        }

        private void SendNotificationEvent()
        {
            var unsubscribedEvent = new UserUnsubscribed
                {
                    EmailAddress = EmailAddress
                };
            var queue = MessageQueueFactory.CreateOutbound("unsubscribed-event", MessagePattern.PublishSubscribe);
            queue.Send(new Message
                {
                    Body = unsubscribedEvent
                });
        }
    }
}
