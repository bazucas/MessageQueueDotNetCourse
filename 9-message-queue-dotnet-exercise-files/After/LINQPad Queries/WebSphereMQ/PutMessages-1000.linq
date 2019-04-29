<Query Kind="Statements">
  <GACReference>amqmdnet, Version=7.5.0.2, Culture=neutral, PublicKeyToken=dd3cb1c9aae9ec97</GACReference>
  <Namespace>IBM.WMQ</Namespace>
</Query>

var properties = new Hashtable();
properties.Add(MQC.HOST_NAME_PROPERTY, "127.0.0.1");
properties.Add(MQC.CHANNEL_PROPERTY, "UNSUB.SVRCONN");

var queueManager = new MQQueueManager("SC.UNSUB", properties);
var queue = queueManager.AccessQueue("test", MQC.MQOO_OUTPUT);

var stopwatch = Stopwatch.StartNew();
for (int i=0; i<1000; i++)
{
var message = new MQMessage();
message.Format = MQC.MQFMT_STRING;
message.WriteString("message " + i.ToString());

queue.Put(message);
}
stopwatch.ElapsedMilliseconds.Dump("No syncpoint");

stopwatch = Stopwatch.StartNew();
for (int i=0; i<1000; i++)
{
var message = new MQMessage();
message.Format = MQC.MQFMT_STRING;
message.WriteString("message " + i.ToString());

var options = new MQPutMessageOptions()
{
	Options = MQC.MQPMO_SYNCPOINT
};
queue.Put(message, options);
queueManager.Commit();
}
stopwatch.ElapsedMilliseconds.Dump("Syncpoint commit per message");

stopwatch = Stopwatch.StartNew();
for (int i=0; i<1000; i++)
{
var message = new MQMessage();
message.Format = MQC.MQFMT_STRING;
message.WriteString("message " + i.ToString());

var options = new MQPutMessageOptions()
{
	Options = MQC.MQPMO_SYNCPOINT
};
queue.Put(message, options);
}
queueManager.Commit();
stopwatch.ElapsedMilliseconds.Dump("Syncpoint commit per batch");

//durable

stopwatch = Stopwatch.StartNew();
for (int i=0; i<1000; i++)
{
var message = new MQMessage();
message.Format = MQC.MQFMT_STRING;
message.WriteString("message " + i.ToString());
message.Persistence = 1;
queue.Put(message);
}
stopwatch.ElapsedMilliseconds.Dump("Durable - No syncpoint");

stopwatch = Stopwatch.StartNew();
for (int i=0; i<1000; i++)
{
var message = new MQMessage();
message.Format = MQC.MQFMT_STRING;
message.WriteString("message " + i.ToString());
message.Persistence = 1;
var options = new MQPutMessageOptions()
{
	Options = MQC.MQPMO_SYNCPOINT
};
queue.Put(message, options);
queueManager.Commit();
}
stopwatch.ElapsedMilliseconds.Dump("Durable - Syncpoint commit per message");

stopwatch = Stopwatch.StartNew();
for (int i=0; i<1000; i++)
{
var message = new MQMessage();
message.Format = MQC.MQFMT_STRING;
message.WriteString("message " + i.ToString());
message.Persistence = MQC.MQPER_PERSISTENT; //1
var options = new MQPutMessageOptions()
{
	Options = MQC.MQPMO_SYNCPOINT
};
queue.Put(message, options);
}
queueManager.Commit();
stopwatch.ElapsedMilliseconds.Dump("Durable - Syncpoint commit per batch");
