<Query Kind="Statements">
  <Reference>clrzmq.dll</Reference>
  <Namespace>ZMQ</Namespace>
</Query>

var context = new Context();
var socket = context.Socket(SocketType.REQ);
socket.Connect("tcp://localhost:5556");
var stopwatch = Stopwatch.StartNew();
for (int i=0; i<10; i++)
{
	socket.Send("A Message: " + i, Encoding.UTF8);
	socket.Send("A2 Message: " + i, Encoding.UTF8);
	var response = socket.Recv(Encoding.UTF8);
	response.Dump();
}
stopwatch.ElapsedMilliseconds.Dump("ZeroMQ REQ: ");
