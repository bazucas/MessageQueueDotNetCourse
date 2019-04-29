<Query Kind="Statements">
  <Reference>C:\TFS\psod-msgq-2\Source\M7\working\Scratchpad\CloudMessaging\packages\AWSSDK.2.0.9.0\lib\net45\AWSSDK.dll</Reference>
  <Namespace>Amazon.SQS</Namespace>
  <Namespace>Amazon</Namespace>
  <Namespace>Amazon.SQS.Model</Namespace>
</Query>

var sqsClient = new AmazonSQSClient("YOUR-ACCESS-KEY", "YOUR-SECRET-KEY", RegionEndpoint.EUWest1);

var stopwatch = Stopwatch.StartNew();
for (int i=0; i<1000; i++)
{
	var request = new SendMessageRequest();
	request.MessageBody = "message " + i.ToString();
	request.QueueUrl = "YOUR-QUEUE-URL";
	sqsClient.SendMessage(request);
}

stopwatch.ElapsedMilliseconds.Dump();