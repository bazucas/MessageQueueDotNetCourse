using Sixeyed.MessageQueue.Integration.Workflows;
using Sixeyed.MessageQueue.Messages.Commands;
using Sixeyed.MessageQueue.Messages.Queries;
using Sixeyed.MessageQueue.Messaging;
using System.Web.Mvc;

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
            if (DoesUserExist(emailAddress))
            {
                StartUnsubscribe(emailAddress);
                return View("Confirmation");
            }
            return View("Unknown");
        }

        private bool DoesUserExist(string emailAddress)
        {
            var exists = false;

            var doesUserExistRequest = new DoesUserExistRequest
                {
                    EmailAddress = emailAddress
                };

            var queue = MessageQueueFactory.CreateOutbound("doesuserexist", MessagePattern.RequestResponse);
            var responseQueue = queue.GetResponseQueue();

            queue.Send(new Message
                {
                    Body = doesUserExistRequest,
                    ResponseAddress = responseQueue.Address
                });
            responseQueue.Receive(x => exists = x.BodyAs<DoesUserExistResponse>().Exists, 5000);
            MessageQueueFactory.Delete(responseQueue);
            
            return exists;
        }


        private static void StartUnsubscribe(string emailAddress)
        {
            var unsubscribeCommand = new UnsubscribeCommand
                {
                    EmailAddress = emailAddress
                };
            var queue = MessageQueueFactory.CreateOutbound("unsubscribe", MessagePattern.FireAndForget);
            queue.Send(new Message
                {
                    Body = unsubscribeCommand
                });
        }

        public ActionResult SubmitSync(string emailAddress)
        {
            var workflow = new UnsubscribeWorkflow(emailAddress);
            workflow.Run();
            return View("Confirmation");
        }
    }
}