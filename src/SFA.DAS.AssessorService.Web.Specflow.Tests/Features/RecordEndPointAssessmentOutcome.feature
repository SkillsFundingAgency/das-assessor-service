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
|	Hawkins	| 		1111111111		|


Scenario: ON- 161.1 Scenario 1 - start recording assessment button
Given I'm on the "Confirm apprentice" page 
When I click on the "start recording assessment" button
Then I should be taken to the "what grade did the apprentice achieve?" page

#And User is on the approved apprenticeship standard list
#When User clicks on continue button
#Then User should be navigated to confirm apprentice page 
#When User clicks on start recording assessment


