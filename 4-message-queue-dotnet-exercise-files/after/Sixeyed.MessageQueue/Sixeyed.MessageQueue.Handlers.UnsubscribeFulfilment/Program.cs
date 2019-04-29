using Sixeyed.MessageQueue.Integration.Workflows;
using Sixeyed.MessageQueue.Messages.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using msmq = System.Messaging;
using Sixeyed.MessageQueue.Messages;

namespace Sixeyed.MessageQueue.Handlers.UnsubscribeFulfilment
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var queue = new msmq.MessageQueue(
                ".\\private$\\sixeyed.messagequeue.unsubscribe-fulfilment"))
            {
                queue.MulticastAddress = "234.1.1.2:8001";
                while (true)
                {
                    Console.WriteLine("unsubscribe-fulfilment listening");
                    var message = queue.Receive();
                    var body = message.BodyStream.ReadFromJson(message.Label);
                    if (body.GetType() == typeof(UserUnsubscribed))
                    {
                        var unsubscribedEvent = body as UserUnsubscribed;
                        Console.WriteLine("Received UserUnsubscribed event for: {0}, at: {1}", unsubscribedEvent.EmailAddress, DateTime.Now);
                        var workflow = new UnsubscribeFulfilmentWorkflow(unsubscribedEvent.EmailAddress);
                        workflow.Run();
                        Console.WriteLine("Processed UserUnsubscribed event for: {0}, at: {1}", unsubscribedEvent.EmailAddress, DateTime.Now);
                    }
                }
            }
        }
    }
}
