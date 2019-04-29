using Newtonsoft.Json;
using Sixeyed.MessageQueue.Integration.Workflows;
using Sixeyed.MessageQueue.Messages.Commands;
using System;
using System.IO;
using msmq = System.Messaging;

namespace Sixeyed.MessageQueue.Handler
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var queue = new msmq.MessageQueue(".\\private$\\sixeyed.messagequeue.unsubscribe-tx"))
            {
                while (true)
                {
                    Console.WriteLine("Listening");
                    using (var tx = new msmq.MessageQueueTransaction())
                    {
                        tx.Begin();
                        var message = queue.Receive(tx);
                        var bodyReader = new StreamReader(message.BodyStream);
                        var jsonBody = bodyReader.ReadToEnd();
                        var unsubscribeMessage = JsonConvert.DeserializeObject<UnsubscribeCommand>(jsonBody);
                        var workflow = new UnsubscribeWorkflow(unsubscribeMessage.EmailAddress);
                        Console.WriteLine("Starting unsubscribe for: {0}, at: {1}", unsubscribeMessage.EmailAddress, DateTime.Now.TimeOfDay);
                        workflow.Run();
                        Console.WriteLine("Unsubscribe complete for: {0}, at: {1}", unsubscribeMessage.EmailAddress, DateTime.Now.TimeOfDay);
                        tx.Commit();
                    }
                }
            }
        }
    }
}
