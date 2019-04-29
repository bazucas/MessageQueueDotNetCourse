<Query Kind="Statements">
  <Reference>clrzmq.dll</Reference>
  <Namespace>ZMQ</Namespace>
</Query>

var context = new Context();
var socket = context.Socket(SocketType.PUSH);
socket.Connect("tcp://localhost:5555");
var stopwatch = Stopwatch.StartNew();
for (int i=0; i<10; i++)
{
	socket.Send("Message: " + i, Encoding.UTF8);
}
stopwatch.ElapsedMilliseconds.Dump("ZeroMQ PUSH"); 