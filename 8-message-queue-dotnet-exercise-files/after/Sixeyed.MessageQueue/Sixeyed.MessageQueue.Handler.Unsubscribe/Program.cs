using Sixeyed.MessageQueue.Integration.Workflows;
using Sixeyed.MessageQueue.Messages.Events;
using Sixeyed.MessageQueue.Messaging;
using System;
using System.Threading;

namespace Sixeyed.MessageQueue.Handler.Unsubscribe
{
    internal class Program
    {
        private static CancellationTokenSource _CancellationTokenSource;

        private static void Main(string[] args)
        {
            Console.WriteLine("Starting.");
            var command = "";
            while (command != "x")
            {
                Start(args);
                Console.WriteLine("Running. Any key to pause, x to exit");
                command = Console.ReadLine();
                _CancellationTokenSource.Cancel();
                if (command != "x")
                {
                    Console.WriteLine("Paused. Any key to resume, x to exit");
                    command = Console.ReadLine();
                }
            }
        }

        private static void Start(string[] args)
        {
            _CancellationTokenSource = new CancellationTokenSource();
            switch (args[0])
            {
                case "unsubscribe-legacy":
                    StartListening<UnsubscribeLegacyWorkflow>("unsubscribe-legacy", MessagePattern.PublishSubscribe);
                    break;

                case "unsubscribe-crm":
                    StartListening<UnsubscribeCrmWorkflow>("unsubscribe-crm", MessagePattern.PublishSubscribe);
                    break;

                case "unsubscribe-fulfilment":
                    StartListening<UnsubscribeFulfilmentWorkflow>("unsubscribe-fulfilment",
                                                                  MessagePattern.PublishSubscribe);
                    break;
            }
        }

        private static void StartListening<TWorkflow>(string name, MessagePattern pattern)
            where TWorkflow : IUserWorkflow, new()
        {
            var queue = MessageQueueFactory.CreateInbound(name, pattern);
            Console.WriteLine("Listening on: {0}", queue.Address);
            queue.Listen(x =>
            {
                if (x.BodyType == typeof(UserUnsubscribed))
                {
                    var unsubscribedEvent = x.BodyAs<UserUnsubscribed>();
                    Console.WriteLine("Starting {0} for: {1}, at: {2}", typeof(TWorkflow).Name,
                                      unsubscribedEvent.EmailAddress, DateTime.Now.TimeOfDay);
                    var workflow = new TWorkflow
                    {
                        EmailAddress = unsubscribedEvent.EmailAddress
                    };
                    workflow.Run();
                    Console.WriteLine("Completed {0} for: {1}, at: {2}", typeof(TWorkflow).Name,
                                      unsubscribedEvent.EmailAddress, DateTime.Now.TimeOfDay);
                }
            }, _CancellationTokenSource.Token);
        }
    }   
}
