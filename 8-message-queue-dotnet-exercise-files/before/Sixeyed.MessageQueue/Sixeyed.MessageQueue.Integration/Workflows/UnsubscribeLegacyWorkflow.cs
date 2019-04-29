using Sixeyed.MessageQueue.Integration.Data;
using System;
using System.Linq;
using System.Threading;

namespace Sixeyed.MessageQueue.Integration.Workflows
{
    public class UnsubscribeLegacyWorkflow : IUserWorkflow
    {
        public string EmailAddress { get; set; }

        public void Run()
        {
            Thread.Sleep(UnsubscribeWorkflow.StepDuration);
            using (var context = new UserModelContainer())
            {
                var user = context.Users.Single(x => x.EmailAddress == EmailAddress);
                user.UserEvents.Add(new UserEvent
                {
                    EventCode = "UNSUB-LEGACY",
                    RecordedAt = DateTime.Now
                });
                context.SaveChanges();
            }
        }
    }
}
