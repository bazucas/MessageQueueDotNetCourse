using System.Collections.Generic;
using Sixeyed.MessageQueue.Integration.Workflows;
using Sixeyed.MessageQueue.Messages.Events;
using Sixeyed.MessageQueue.Messaging;
using System;

namespace Sixeyed.MessageQueue.Handlers.UnsubscribeCrm
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var queueAddress = ".\\private$\\sixeyed.messagequeue.unsubscribe-crm";
            var properties = new Dictionary<string, object>();
            properties.Add("MulticastAddress", "234.1.1.2:8001");
            using (var queue = MessageQueueFactory.CreateInbound(queueAddress, MessagePattern.PublishSubscribe, properties))
            {
                Console.WriteLine("Listening on: {0}", queueAddress);
                queue.Listen(x =>
                    {
                        if (x.BodyType == typeof (UserUnsubscribed))
                        {
                            var unsubscribedEvent = x.BodyAs<UserUnsubscribed>();
                            Console.WriteLine("Starting UnsubscribeCrmWorkflow for: {0}, at: {1}",
                                              unsubscribedEvent.EmailAddress, DateTime.Now.TimeOfDay);
                            var workflow = new UnsubscribeCrmWorkflow(unsubscribedEvent.EmailAddress);
                            workflow.Run();
                            Console.WriteLine("Completed UnsubscribeCrmWorkflow for: {0}, at: {1}",
                                              unsubscribedEvent.EmailAddress, DateTime.Now.TimeOfDay);
                        }
                    });
            }
        }
    }
}