<Query Kind="Statements">
  <Reference>C:\TFS\psod-msgq-2\Source\M7\working\Scratchpad\CloudMessaging\packages\WindowsAzure.ServiceBus.2.2.6.0\lib\net40-full\Microsoft.ServiceBus.dll</Reference>
  <Namespace>Microsoft.ServiceBus.Messaging</Namespace>
</Query>

var factory = MessagingFactory.CreateFromConnectionString("YOUR-CONNECTION-STRING");
var queueClient = factory.CreateQueueClient("test");

var stopwatch = Stopwatch.StartNew();
for (int i=0; i<1000; i++)
{
	var message = new BrokeredMessage("message");
	queueClient.Send(message);
}

stopwatch.ElapsedMilliseconds.Dump();