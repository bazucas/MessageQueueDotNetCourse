﻿using Sixeyed.MessageQueue.Integration.Validators;
using Sixeyed.MessageQueue.Integration.Workflows;
using Sixeyed.MessageQueue.Messages.Commands;
using Sixeyed.MessageQueue.Messages.Queries;
using Sixeyed.MessageQueue.Messaging;
using System;

namespace Sixeyed.MessageQueue.Handler
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            switch(args[0])
            {
                case "unsubscribe":
                    StartListening("unsubscribe", MessagePattern.FireAndForget);
                    break;
                case "doesuserexist":
                    StartListening("doesuserexist", MessagePattern.RequestResponse);
                    break;
            }
        }

        private static void StartListening(string name, MessagePattern pattern)
        {
            var queue = MessageQueueFactory.CreateInbound(name, pattern);
            Console.WriteLine("Listening on: {0}", queue.Address);
            queue.Listen(x =>
                {
                    if (x.BodyType == typeof (UnsubscribeCommand))
                    {
                        Unsubscribe(x.BodyAs<UnsubscribeCommand>());
                    }
                    else if (x.BodyType == typeof (DoesUserExistRequest))
                    {
                        CheckUserExists(x.BodyAs<DoesUserExistRequest>(), x, queue);
                    }
                });
        }

        private static void CheckUserExists(DoesUserExistRequest doesUserExistRequest, Message requestMessage,
                                            IMessageQueue requestQueue)
        {
            Console.WriteLine("Starting CheckUserExists for: {0}, at: {1}", doesUserExistRequest.EmailAddress,
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
            Console.WriteLine("Returned: {0} for DoesUserExist, EmailAddress: {1}, to: {2}, at: {3}",
                              responseBody.Exists,
                              doesUserExistRequest.EmailAddress, responseQueue.Address, DateTime.Now.TimeOfDay);
        }

        private static void Unsubscribe(UnsubscribeCommand unsubscribeMessage)
        {
            Console.WriteLine("Starting unsubscribe for: {0}, at: {1}", unsubscribeMessage.EmailAddress,
                              DateTime.Now.TimeOfDay);
            var workflow = new UnsubscribeWorkflow(unsubscribeMessage.EmailAddress);
            workflow.Run();
            Console.WriteLine("Unsubscribe complete for: {0}, at: {1}", unsubscribeMessage.EmailAddress,
                              DateTime.Now.TimeOfDay);
        }
    }
}