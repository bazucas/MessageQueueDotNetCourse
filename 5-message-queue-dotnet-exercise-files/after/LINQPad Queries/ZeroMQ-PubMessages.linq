<Query Kind="Statements">
  <Reference>clrzmq.dll</Reference>
  <Namespace>ZMQ</Namespace>
</Query>

var context = new Context();
var socket = context.Socket(SocketType.PUB);
socket.Bind("tcp://*:5557");
var stopwatch = Stopwatch.StartNew();
var i = 0;
while(true)
{
	socket.Send("A Message: " + i, Encoding.UTF8);
	socket.Send("B Message: " + i, Encoding.UTF8);
	Thread.Sleep(100);
	i++;
}
stopwatch.ElapsedMilliseconds.Dump("ZeroMQ PUB: ");