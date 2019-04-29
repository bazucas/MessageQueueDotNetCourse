<Query Kind="Statements">
  <Reference>C:\TFS\psod-msgq-2\Source\M7\working\Scratchpad\CloudMessaging\packages\AWSSDK.2.0.9.0\lib\net45\AWSSDK.dll</Reference>
  <Namespace>Amazon.SQS</Namespace>
  <Namespace>Amazon</Namespace>
  <Namespace>Amazon.SQS.Model</Namespace>
</Query>

var sqsClient = new AmazonSQSClient("YOUR-ACCESS-KEY", "YOUR-SECRET-KEY", RegionEndpoint.EUWest1);

var request = new ReceiveMessageRequest();
request.QueueUrl = "YOUR-QUEUE-URL";
var response = sqsClient.ReceiveMessage(request);
response.Dump();

var deleteRequest = new DeleteMessageRequest();
deleteRequest.QueueUrl = request.QueueUrl;
deleteRequest.ReceiptHandle = response.Messages[0].ReceiptHandle;

var deleteResponse = sqsClient.DeleteMessage(deleteRequest);
deleteResponse.Dump();