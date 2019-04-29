using Sixeyed.MessageQueue.Integration.Workflows;
using Sixeyed.MessageQueue.Messages;
using Sixeyed.MessageQueue.Messages.Commands;
using System.Web.Mvc;
using msmq = System.Messaging;

namespace Sixeyed.MessageQueue.Web.Controllers
{
    public class UnsubscribeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Submit(string emailAddress)
        {
            StartUnsubscribe(emailAddress);
            return View("Confirmation");
        }

        private static void StartUnsubscribe(string emailAddress)
        {
            var unsubscribeCommand = new UnsubscribeCommand
            {
                EmailAddress = emailAddress
            };
            using (var queue = new msmq.MessageQueue(
                ".\\private$\\sixeyed.messagequeue.unsubscribe"))
            {
                var message = new msmq.Message();
                message.BodyStream = unsubscribeCommand.ToJsonStream();
                message.Label = unsubscribeCommand.GetMessageType();
                queue.Send(message);
            }
        }

        public ActionResult SubmitSync(string emailAddress)
        {
            var workflow = new UnsubscribeWorkflow(emailAddress);
            workflow.Run();
            return View("Confirmation");
        }
	}
}