using Sixeyed.MessageQueue.Integration.Workflows;
using Sixeyed.MessageQueue.Messages;
using Sixeyed.MessageQueue.Messages.Commands;
using Sixeyed.MessageQueue.Messages.Queries;
using System;
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
            if (DoesUserExist(emailAddress))
            {
                StartUnsubscribe(emailAddress);
                return View("Confirmation");
            }
            return View("Unknown");
        }

        private bool DoesUserExist(string emailAddress)
        {
            var responseAddress = Guid.NewGuid().ToString().Substring(0, 6);
            responseAddress = ".\\private$\\" + responseAddress;
            try
            {
                using (var responseQueue = msmq.MessageQueue.Create(responseAddress))
                {
                    var doesUserExistRequest = new DoesUserExistRequest
                    {
                        EmailAddress = emailAddress
                    };
                    using (var requestQueue = new msmq.MessageQueue(
                        ".\\private$\\sixeyed.messagequeue.doesuserexist"))
                    {
                        var message = new msmq.Message();
                        message.BodyStream = doesUserExistRequest.ToJsonStream();
                        message.Label = doesUserExistRequest.GetMessageType();
                        message.ResponseQueue = responseQueue;
                        requestQueue.Send(message);
                    }
                    var response = responseQueue.Receive();
                    var responseBody = response.BodyStream.ReadFromJson<DoesUserExistResponse>();
                    return responseBody.Exists;
                }
            }
            finally
            {
                if (msmq.MessageQueue.Exists(responseAddress))
                {
                    msmq.MessageQueue.Delete(responseAddress);
                }
            }
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