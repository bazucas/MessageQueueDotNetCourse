using System.Threading;

namespace Sixeyed.MessageQueue.Integration.Workflows
{
    public  class UnsubscribeLegacyWorkflow
    {
        public string EmailAddress { get; private set; }

        public UnsubscribeLegacyWorkflow(string emailAddress)
        {
            EmailAddress = emailAddress;
        }

        public void Run()
        {
            Thread.Sleep(UnsubscribeWorkflow.StepDuration);
        }
    }
}
