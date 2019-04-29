using Sixeyed.MessageQueue.Integration.Workflows;
using Sixeyed.MessageQueue.Messages;
using Sixeyed.MessageQueue.Messages.Commands;
using System;
using msmq = System.Messaging;

namespace Sixeyed.MessageQueue.Handler
{
    class Program
    {
        static void Main(string[] args)
        {
            var queueAddress = args != null && args.Length == 1 ? args[0] :
                ".\\private$\\sixeyed.messagequeue.unsubscribe";

            using (var queue = new msmq.MessageQueue(queueAddress))
            {
                while (true)
                {
                    Console.WriteLine("Listening on: {0}", queueAddress);
                    var message = queue.Receive();
                    var messageBody = message.BodyStream.ReadFromJson(message.Label);
                    //TODO - would use a factory/IoC/MEF for this:
                    if (messageBody.GetType() == typeof(UnsubscribeCommand))
                    {
                        Unsubscribe((UnsubscribeCommand)messageBody);
                    }
                }
            }
        }

        private static void Unsubscribe(UnsubscribeCommand unsubscribeMessage)
        {
            Console.WriteLine("Starting unsubscribe for: {0}, at: {1}", unsubscribeMessage.EmailAddress, DateTime.Now.TimeOfDay);
            var workflow = new UnsubscribeWorkflow(unsubscribeMessage.EmailAddress);
            workflow.Run();
            Console.WriteLine("Unsubscribe complete for: {0}, at: {1}", unsubscribeMessage.EmailAddress, DateTime.Now.TimeOfDay);
        }
    }
}
