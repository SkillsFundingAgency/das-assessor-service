@queryOrganisations
Feature: Query Organisations through the SFA.DAS.AssessorService.Application.Api
	In order to be able to get information on Organisations
	As a System
	I want to be be able to query Organisations

Scenario: Retrieve All Organisations
	Given System Has access to the SFA.DAS.AssessmentOrgs.Api	
	When I Request All Organisations to be retrieved
	Then the response http status should be Ok
	And the API returns all Organisations

Scenario: Search for an Organisation using a ukprn set to 10000000
	Given System Has access to the SFA.DAS.AssessmentOrgs.Api	
	When I search for an organisation with its ukprn set to 10000000
	Then the response http status should be Ok
	And the API returns an appropriate Organisation

Scenario: Search for an Organisation using a ukprn set to 12
	Given System Has access to the SFA.DAS.AssessmentOrgs.Api	
	When I search for an organisation with its ukprn set to 12
	Then the response http status should be BadRequest
	And the response message should contain A valid UKPRN as defined in the UK Register of Learning Providers (UKRLP) is 8 digits in the format 10000000 – 99999999

	Scenario: Search for an Organisation using a ukprn which does not exist 
	Given System Has access to the SFA.DAS.AssessmentOrgs.Api	
	When I search for an organisation with its ukprn set to 10029999
	Then the response http status should be Not Found
	And the response message should contain No provider with ukprn 10029999 found