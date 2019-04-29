<Query Kind="Statements">
  <Reference>clrzmq.dll</Reference>
  <Namespace>ZMQ</Namespace>
</Query>

var context = new Context();
var socket = context.Socket(SocketType.SUB);
socket.Connect("tcp://localhost:5557");
socket.Subscribe("B", Encoding.UTF8);
while (true)
{
	var message = socket.Recv(Encoding.UTF8);
	message.Dump();
}