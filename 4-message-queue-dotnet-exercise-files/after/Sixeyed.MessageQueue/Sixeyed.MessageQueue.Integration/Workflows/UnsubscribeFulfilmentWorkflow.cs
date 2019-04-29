using System.Threading;

namespace Sixeyed.MessageQueue.Integration.Workflows
{
    public  class UnsubscribeFulfilmentWorkflow
    {
        public string EmailAddress { get; private set; }

        public UnsubscribeFulfilmentWorkflow(string emailAddress)
        {
            EmailAddress = emailAddress;
        }

        public void Run()
        {
            Thread.Sleep(UnsubscribeWorkflow.StepDuration);
        }
    }
}
