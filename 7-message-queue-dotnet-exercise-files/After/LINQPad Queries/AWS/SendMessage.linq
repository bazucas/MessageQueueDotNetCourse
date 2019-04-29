<Query Kind="Statements">
  <Reference>C:\TFS\psod-msgq-2\Source\M7\working\Scratchpad\CloudMessaging\packages\AWSSDK.2.0.9.0\lib\net45\AWSSDK.dll</Reference>
  <Namespace>Amazon.SQS</Namespace>
  <Namespace>Amazon</Namespace>
  <Namespace>Amazon.SQS.Model</Namespace>
</Query>

var request = new SendMessageRequest();
request.MessageBody = "message";
request.QueueUrl = "YOUR-QUEUE-URL";

var sqsClient = new AmazonSQSClient("YOUR-ACCESS-KEY", "YOUR-SECRET-KEY", RegionEndpoint.EUWest1);

var response = sqsClient.SendMessage(request);
response.Dump();