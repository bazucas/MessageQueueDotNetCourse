using System.Configuration;
using Sixeyed.MessageQueue.Integration.Validators;
using Sixeyed.MessageQueue.Integration.Workflows;
using Sixeyed.MessageQueue.Messages.Commands;
using Sixeyed.MessageQueue.Messages.Events;
using Sixeyed.MessageQueue.Messages.Queries;
using Sixeyed.MessageQueue.Messaging;
using System;
using System.Threading;

namespace Sixeyed.MessageQueue.Handler
{
    public class QueueListener
    {
        private CancellationTokenSource _cancellationTokenSource;

        public void Start(string queueName)
        {
            Log.WriteLine("QueueListener starting...");

            //query to warm up EF:
            var dummy = new UserValidator(".").Exists();

            if (string.IsNullOrEmpty(queueName))
            {
                queueName = ConfigurationManager.AppSettings["listenOnQueueName"];
                if (string.IsNullOrEmpty(queueName))
                {
                    throw new ArgumentException(
                        "'listenOnQueueName:[queueName]' must be specified as an sppSetting or command line argument");
                }
            }

            Log.WriteLine("Starting with queueName: {0}", queueName);

            _cancellationTokenSource = new CancellationTokenSource();
            switch (queueName)
            {
                case "unsubscribe":
                    StartListening("unsubscribe", MessagePattern.FireAndForget);
                    break;

                case "doesuserexist":
                    StartListening("doesuserexist", MessagePattern.RequestResponse);
                    break;

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

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        private void StartListening(string name, MessagePattern pattern)
        {
            var queue = MessageQueueFactory.CreateInbound(name, pattern);
            Log.WriteLine("Listening with queue type: {0}, on address: {1}", queue.GetType().Name, queue.Address);
            queue.Listen(x =>
            {
                if (x.BodyType == typeof(UnsubscribeCommand))
                {
                    Unsubscribe(x.BodyAs<UnsubscribeCommand>());
                }
                else if (x.BodyType == typeof(DoesUserExistRequest))
                {
                    CheckUserExists(x.BodyAs<DoesUserExistRequest>(), x, queue);
                }
            }, _cancellationTokenSource.Token);
        }

        private void StartListening<TWorkflow>(string name, MessagePattern pattern)
           where TWorkflow : IUserWorkflow, new()
        {
            var queue = MessageQueueFactory.CreateInbound(name, pattern);
            Log.WriteLine("Listening with queue type: {0}, on address: {1}", queue.GetType().Name, queue.Address);
            queue.Listen(x =>
            {
                if (x.BodyType == typeof(UserUnsubscribed))
                {
                    var unsubscribedEvent = x.BodyAs<UserUnsubscribed>();
                    Log.WriteLine("Starting {0} for: {1}, at: {2}", typeof(TWorkflow).Name,
                                      unsubscribedEvent.EmailAddress, DateTime.Now.TimeOfDay);
                    var workflow = new TWorkflow
                    {
                        EmailAddress = unsubscribedEvent.EmailAddress
                    };
                    workflow.Run();
                    Log.WriteLine("Completed {0} for: {1}, at: {2}", typeof(TWorkflow).Name,
                                      unsubscribedEvent.EmailAddress, DateTime.Now.TimeOfDay);
                }
            }, _cancellationTokenSource.Token);
        }

        private static void CheckUserExists(DoesUserExistRequest doesUserExistRequest, Message requestMessage,
                                            IMessageQueue requestQueue)
        {
            Log.WriteLine("Starting CheckUserExists for: {0}, at: {1}", doesUserExistRequest.EmailAddress,
                              DateTime.Now.TimeOfDay);
            var validator = new UserValidator(doesUserExistRequest.EmailAddress);
            var responseBody = new DoesUserExistResponse
            {
                Exists = validator.Exists()
            };
            var responseQueue = requestQueue.GetReplyQueue(requestMessage);
            responseQueue.Send(new Message
            {
                Body = responseBody
            });
            Log.WriteLine("Returned: {0} for DoesUserExist, EmailAddress: {1}, to: {2}, at: {3}",
                              responseBody.Exists,
                              doesUserExistRequest.EmailAddress, responseQueue.Address, DateTime.Now.TimeOfDay);
        }

        private static void Unsubscribe(UnsubscribeCommand unsubscribeMessage)
        {
            Log.WriteLine("Starting unsubscribe for: {0}, at: {1}", unsubscribeMessage.EmailAddress,
                              DateTime.Now.TimeOfDay);
            var workflow = new UnsubscribeWorkflow(unsubscribeMessage.EmailAddress);
            workflow.Run();
            Log.WriteLine("Unsubscribe complete for: {0}, at: {1}", unsubscribeMessage.EmailAddress,
                              DateTime.Now.TimeOfDay);
        }
    }
}
