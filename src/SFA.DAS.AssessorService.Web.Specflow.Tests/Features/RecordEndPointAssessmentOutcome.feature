@RecordEndPointAssessmentOutcome
Feature: RecordEndPointAssessmentOutcome
	As an End Point Assessor Organisation
	I want to record the grade achieved by an apprentice 
	So that the information can appear on the certificate

Background:
Given User Logs on through idams
And User enters valid credentials 
And Clicks on sign in button
And User should be navigated to search for an apprentice page on EPAO service
And User enters valid search criteria 
|	lastname	| 		ULN			|
|	Taylor	| 		2222222222		|
And Clicks On Search Apprentice Button


Scenario: ON - 161.1 Scenario 1 - succesfully recording assessment
Given I'm on the Confirm Apprentice Page 
When I click on the Start Recording Assessment Button
Then I should be taken to the What Grade did the Apprentice Achieve page
When The User Selects Grade
And User Clicks On Continue With What Grade
Then User should be navigated to did the apprentice do any additional learning options page
When User Selects No Option
And User Clicks On Continue With Addtional Options
Then User Is on the Apprentice Achievement Date Page
When The User Enters Detials Achievement Date
And Clicks On Continue with Apprentice Detials Achievement Date
Then I should be taken to the Where will the certificate be Sent Page
When I have entered Details for where to send the Certificate
And User Clicks on Continue with the Certificate to be Sent Details
Then I should be navigated to the Check and Approve the Assessment Details Page
When User Confirms Check And Approve Details
Then The User is taken to the Declaraton Page
When The User Conforms And Applys for a Certificate
Then Assessment Is Recorded


