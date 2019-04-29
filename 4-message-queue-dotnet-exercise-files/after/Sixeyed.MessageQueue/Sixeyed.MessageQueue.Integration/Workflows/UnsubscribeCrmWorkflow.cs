using System.Threading;

namespace Sixeyed.MessageQueue.Integration.Workflows
{
    public class UnsubscribeCrmWorkflow
    {
        public string EmailAddress { get; private set; }

        public UnsubscribeCrmWorkflow(string emailAddress)
        {
            EmailAddress = emailAddress;
        }

        public void Run()
        {
            Thread.Sleep(UnsubscribeWorkflow.StepDuration);
        }
    }
}
