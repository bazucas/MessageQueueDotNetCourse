<Query Kind="Statements">
  <Reference>C:\TFS\psod-msgq-2\Source\M7\working\Scratchpad\CloudMessaging\packages\WindowsAzure.ServiceBus.2.2.6.0\lib\net40-full\Microsoft.ServiceBus.dll</Reference>
  <Namespace>Microsoft.ServiceBus.Messaging</Namespace>
</Query>

var factory = MessagingFactory.CreateFromConnectionString("YOUR-CONNECTION-STRING");
var queueClient = factory.CreateQueueClient("test");

var message = queueClient.Receive();
message.GetBody<string>().Dump("Body");

message.Complete();