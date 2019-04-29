Feature: Unsubscribe
	In order to stop receiving spam
	As a website user
	I want to be able to unsubscribe from all mailing lists

Scenario: Unsubscribe for known user
	Given the message handlers are running 
	When a user submits the unsubscribe form with email address elton@sixeyed.com
	Then the user will receive a Confirmation response
	And they should be flagged in the database as unsubscribed within 5 seconds
	And they should be unsubscribed from the legacy system within 5 seconds
	And they should be unsubscribed from CRM within 5 seconds
	And they should be unsubscribed from the mail fulfilment system within 5 seconds

