<Query Kind="Statements">
  <Reference>&lt;RuntimeDirectory&gt;\System.Messaging.dll</Reference>
  <Namespace>System.Messaging</Namespace>
</Query>

MessageQueue queue;
Stopwatch stopwatch;
MessageQueueTransaction tx;

queue = new MessageQueue(@".\private$\default-queue");
stopwatch = Stopwatch.StartNew();
for (int i=0; i<1000; i++)
{
	queue.Send("Message: " + i);
}
stopwatch.ElapsedMilliseconds.Dump("default-queue: ");

queue = new MessageQueue(@".\private$\durable-queue");
queue.DefaultPropertiesToSend.Recoverable= true;
stopwatch = Stopwatch.StartNew();
for (int i=0; i<1000; i++)
{
	queue.Send("Message: " + i);
}
stopwatch.ElapsedMilliseconds.Dump("durable-queue: ");

queue = new MessageQueue(@".\private$\transactional-queue");
stopwatch = Stopwatch.StartNew();
for (int i=0; i<1000; i++)
{
	tx = new MessageQueueTransaction();
	tx.Begin();
	queue.Send("Message: " + i, tx);
	tx.Commit();
}
stopwatch.ElapsedMilliseconds.Dump("transactional-queue: ");

queue = new MessageQueue(@".\private$\transactional-queue");
stopwatch = Stopwatch.StartNew();
tx = new MessageQueueTransaction();
tx.Begin();
for (int i=0; i<1000; i++)
{
	queue.Send("Message: " + i, tx);
}
tx.Commit();
stopwatch.ElapsedMilliseconds.Dump("transactional-queue (batched): ");