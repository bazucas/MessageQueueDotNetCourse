using Newtonsoft.Json;
using Sixeyed.MessageQueue.Integration.Workflows;
using Sixeyed.MessageQueue.Messages.Commands;
using System;
using System.IO;
using System.Text;
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
            var workflow = new UnsubscribeWorkflow(emailAddress);
            workflow.Run();
            return View("Confirmation");
        }
	}
}