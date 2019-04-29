<Query Kind="Statements">
  <Reference>&lt;RuntimeDirectory&gt;\System.Messaging.dll</Reference>
  <Namespace>System.Messaging</Namespace>
</Query>

MessageQueue.Create(@".\private$\default-queue");

MessageQueue.Create(@".\private$\transactional-queue", true);

MessageQueue.Create(@".\private$\secure-queue", true);
var secureQueue = new MessageQueue(@".\private$\secure-queue");
secureQueue.Authenticate = true;
secureQueue.EncryptionRequired = EncryptionRequired.Body;

MessageQueue.Create(@".\private$\durable-queue");