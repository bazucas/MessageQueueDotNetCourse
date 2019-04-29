<Query Kind="Statements">
  <Reference>&lt;RuntimeDirectory&gt;\System.Messaging.dll</Reference>
  <Namespace>System.Messaging</Namespace>
</Query>

var queue = new MessageQueue(@".\private$\secure-queue");
queue.DefaultPropertiesToSend.UseEncryption = true;
//queue.DefaultPropertiesToSend.UseAuthentication = true;
var stopwatch = Stopwatch.StartNew();
for (int i=0; i<1000; i++)
{
	queue.Send("Message: " + i);
}
stopwatch.ElapsedMilliseconds.Dump("secure-queue: ");