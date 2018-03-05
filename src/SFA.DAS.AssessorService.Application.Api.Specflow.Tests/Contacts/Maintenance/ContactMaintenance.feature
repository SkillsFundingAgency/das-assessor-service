@maintainContacts
Feature: Maintain Contacts through the SFA.DAS.AssessorService.Application.Api
	In order to be able to Modify Contacts
	As a System
	I want to be be able to maintain Contacts

Scenario: Create a Contact as First User for Organisation
	Given System Has access to the SFA.DAS.AssessmentOrgs.Api	
	When I Create a Contact as First User for Organisation
	| UserName | DisplayName | Email | EndPointAssessorOrganisationId | EndPointAssessorUKPRN |
	| testuser | Test        | jane128@gmail.com             | 99998888      | 10038887              |
	Then the response http status should be Created
	And the Location Header should be set
	And the Contact should be created
	And the Contact Status should be set to Live
	And the Contact Organisation Status should be set to Live

Scenario: Create a Contact when other Contact Exist for Organisation
	Given System Has access to the SFA.DAS.AssessmentOrgs.Api	
	When I Create a Contact as another User for Organisation
	| UserName | DisplayName | Email | EndPointAssessorOrganisationId | EndPointAssessorUKPRN |
	| testuser140 | Test        | jane1289@gmail.com             | 9999887      | 10038887              |
	Then the response http status should be Created
	And the Location Header should be set
	And the Contact should be created
	And the Contact Status should be set to Live
	And the Contact Organisation Status should be set to Live

Scenario: Create a Contact When already Exists
	Given System Has access to the SFA.DAS.AssessmentOrgs.Api	
	When I Create a Contact That already exists
	| UserName | DisplayName | Email | EndPointAssessorOrganisationId | EndPointAssessorUKPRN |
	| testuser | Jean Brodie | jbrodie@gmail.com | 99998899      | 10038887              |
	Then the response http status should be Bad Request

Scenario: Update a Contact succesfully
	Given System Has access to the SFA.DAS.AssessmentOrgs.Api	
	When I Update a Contact succesfully
	| UserName | DisplayName | Email |
	| testuser | Test        | jane1289@gmail.com             |
	Then the response http status should be No Content
	And the Contact Update should have occured

Scenario: Delete a Contact 
	Given System Has access to the SFA.DAS.AssessmentOrgs.Api	
	When I Delete a Contact
	| UserName | DisplayName | Email | EndPointAssessorOrganisationId | EndPointAssessorUKPRN |
	| testuser123 | Jean Brodie | jbrodie@gmail.com | 99998899      | 10038887              |
	Then the response http status should be No Content
	And the Contact should be deleted

Scenario: Repeat Deleting a Contact 
	Given System Has access to the SFA.DAS.AssessmentOrgs.Api	
	When I Delete a Contact Twice
	| UserName | DisplayName | Email | EndPointAssessorOrganisationId | EndPointAssessorUKPRN |
	| testuser127 | Jean Brodie | jbrodie@gmail.com | 99998899      | 10038887              |
	Then the response http status should be Not Found
	And the Contact should be deleted
	