using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sixeyed.MessageQueue.Integration.Data;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using TechTalk.SpecFlow;

namespace Sixeyed.MessageQueue.EndToEnd.Tests
{
    [Binding]
    public class UnsubscribeSteps
    {
        private string _emailAddress;
        private DateTime _testStartedAt;
        private string _htmlResponse;

        [Given(@"the message handlers are running")]
        public void GivenTheMessageHandlersAreRunning()
        {
            var processes = Process.GetProcessesByName("Sixeyed.MessageQueue.Handler");
            Assert.IsTrue(processes.Any());
        }

        [When(@"a user submits the unsubscribe form with email address (.*)")]
        public void WhenAUserSubmitsTheUnsubscribeFormWithEmailAddressEltonSixeyed_Com(string emailAddress)
        {
            _testStartedAt = DateTime.Now;
            _emailAddress = emailAddress;
            var url = "http://localhost/Sixeyed.MessageQueue.Web/Unsubscribe/Submit";
            var client = new WebClient();
            var values = new NameValueCollection();
            values.Add("emailAddress", _emailAddress);
            var response = client.UploadValues(url, values);
            _htmlResponse = Encoding.Default.GetString(response);
        }

        [Then(@"the user will receive a Confirmation response")]
        public void ThenTheUserWillReceiveAConfirmationResponse()
        {
            Assert.IsTrue(_htmlResponse.Contains("<title>Confirmation"));
        }

        [Then(@"the user will receive an Unknown User response")]
        public void ThenTheUserWillReceiveAnUnknownUserResponse()
        {
            Assert.IsTrue(_htmlResponse.Contains("<title>Unknown"));
        }

        [Then(@"they should be flagged in the database as unsubscribed within (.*) seconds")]
        public void ThenTheyShouldBeFlaggedInTheDatabaseAsUnsubscribedWithinSeconds(int timeoutSeconds)
        {
            AssertUserEvent("UNSUB", timeoutSeconds);
        }

        [Then(@"they should be unsubscribed from the legacy system within (.*) seconds")]
        public void ThenTheyShouldBeUnsubscribedFromTheLegacySystemWithinSeconds(int timeoutSeconds)
        {
            AssertUserEvent("UNSUB-LEGACY", timeoutSeconds);
        }

        [Then(@"they should be unsubscribed from CRM within (.*) seconds")]
        public void ThenTheyShouldBeUnsubscribedFromCRMWithinSeconds(int timeoutSeconds)
        {
            AssertUserEvent("UNSUB-CRM", timeoutSeconds);
        }

        [Then(@"they should be unsubscribed from the mail fulfilment system within (.*) seconds")]
        public void ThenTheyShouldBeUnsubscribedFromTheMailFulfilmentSystemWithinSeconds(int timeoutSeconds)
        {
            AssertUserEvent("UNSUB-FULFIL", timeoutSeconds);
        }

        private void AssertUserEvent(string expectedEventCode, int timeoutSeconds)
        {

            RetryAssert.WithinTimeout(() =>
            {
                using (var context = new UserModelContainer())
                {
                    return context.UserEvents.Count(x => x.User.EmailAddress == _emailAddress &&
                        x.EventCode == expectedEventCode &&
                        x.RecordedAt > _testStartedAt) == 1;
                }
            }, 250, 1000 * timeoutSeconds, "Expected event code: {0} recorded after: {1}", expectedEventCode, _testStartedAt);   
        }
    }
}
