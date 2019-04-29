<Query Kind="Statements">
  <Reference>&lt;RuntimeDirectory&gt;\System.Messaging.dll</Reference>
  <Namespace>System.Messaging</Namespace>
</Query>


var queues = new List<string> 
{
	@".\private$\default-queue",
	@".\private$\transactional-queue",
	@".\private$\secure-queue",
	@".\private$\durable-queue" 
};

queues.ForEach(x=> {
	var queue = new MessageQueue(x);
	queue.Purge();
});
