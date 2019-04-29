<Query Kind="Statements">
  <Reference>&lt;RuntimeDirectory&gt;\System.Messaging.dll</Reference>
  <Namespace>System.Messaging</Namespace>
</Query>


using (var queue = new MessageQueue("FormatName:MULTICAST=234.1.1.1:8001"))
{
	queue.DefaultPropertiesToSend.Recoverable = true;
	queue.Send(string.Format("Publishing at: {0}", DateTime.Now));
};