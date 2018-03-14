@queryContacts
Feature: Query Contacts through the SFA.DAS.AssessorService.Application.Api
	In order to be able to get information on Contacts
	As a System
	I want to be be able to query Contacts

Scenario: Retrieve All Contacts for an Organisation
	Given System Has access to the SFA.DAS.AssessmentOrgs.Api	
	When I Request All Contacts to be retrieved BY Organisation with Id 10033670
	Then the response http status should be OK
	And the API returns all Contacts for an Organisation

Scenario: Search Contact for an Organisation By Username
	Given System Has access to the SFA.DAS.AssessmentOrgs.Api	
	When Client Searches Contacts By Username
	| username | emailaddress |
	|jcoxhead|jcoxhead@gmail.com|
	Then the response http status should be OK
	And the API returns a valid Contact

