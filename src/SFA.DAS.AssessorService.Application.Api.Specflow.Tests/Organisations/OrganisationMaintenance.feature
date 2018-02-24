@maintainOrganisations
Feature: Maintain Organisations through the SFA.DAS.AssessorService.Application.Api
	In order to be able to Modify Organisations
	As a System
	I want to be be able to maintain Organisations

Scenario: Create an Organisation
	Given System Has access to the SFA.DAS.AssessmentOrgs.Api	
	When I Create an Organisation
	Then the response http status should be Created
	And the Location Header should be set
	And the Organisation should be created
	

