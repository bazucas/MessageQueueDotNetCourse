<Query Kind="Statements">
  <GACReference>amqmdnet, Version=7.5.0.2, Culture=neutral, PublicKeyToken=dd3cb1c9aae9ec97</GACReference>
  <Namespace>IBM.WMQ</Namespace>
</Query>

var properties = new Hashtable();
properties.Add(MQC.HOST_NAME_PROPERTY, "127.0.0.1");
properties.Add(MQC.CHANNEL_PROPERTY, "UNSUB.SVRCONN");

var queueManager = new MQQueueManager("SC.UNSUB", properties);
var queue = queueManager.AccessQueue("test", MQC.MQOO_INPUT_AS_Q_DEF);

var message = new MQMessage();
message.Format = MQC.MQFMT_STRING;

queue.Get(message);

message.ReadString(message.MessageLength).Dump("Body");