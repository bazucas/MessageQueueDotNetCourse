<Query Kind="Statements">
  <Reference>clrzmq.dll</Reference>
  <Namespace>ZMQ</Namespace>
</Query>

var context = new Context();
var socket = context.Socket(SocketType.PULL);
socket.Bind("tcp://*:5555");
while (true)
{
	var message = socket.Recv(Encoding.UTF8);
	message.Dump();
	socket.Send("Received", Encoding.UTF8); 
}