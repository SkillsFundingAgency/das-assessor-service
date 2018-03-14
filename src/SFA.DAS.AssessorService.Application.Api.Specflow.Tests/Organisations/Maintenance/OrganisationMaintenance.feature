@maintainOrganisations
Feature: Maintain Organisations through the SFA.DAS.AssessorService.Application.Api
	In order to be able to Modify Organisations
	As a System
	I want to be be able to maintain Organisations

Scenario: Create an Organisation With No Primary Contact
	Given System Has access to the SFA.DAS.AssessmentOrgs.Api	
	When I Create an Organisation
	| EndPointAssessorName | EndPointAssessorOrganisationId | EndPointAssessorUKPRN |
	| Test                 | 99999999                       | 10033333              |
	Then the response http status should be Created
	And the Location Header should be set
	And the Organisation should be created
	And the Organisation Status should be set to New

Scenario: Create an Organisation With Primary Contact that exists
	Given System Has access to the SFA.DAS.AssessmentOrgs.Api	
	 When I Create an Organisation With Existing Primary Contact
	| EndPointAssessorName | EndPointAssessorOrganisationId | EndPointAssessorUKPRN |
	| Test                 | 99999987                      | 10033333              |
	Then the response http status should be Created
	And the Location Header should be set
	And the Organisation should be created
	And the Organisation Status should be set to Live
                
Scenario: Create an Organisation With Invalid UkPrn
	Given System Has access to the SFA.DAS.AssessmentOrgs.Api	
    When I Create an Organisation
	| EndPointAssessorName | EndPointAssessorOrganisationId | EndPointAssessorUKPRN |
	| Test                 | 99999998                       | 14              |
	Then the response http status should be Bad Request
	And the response message should contain Request must contain a valid UKPRN as defined in the UK Register of Learning Providers (UKRLP) is 8 digits in the format 10000000 – 99999999
	
Scenario: Create an Organisation Which Already Exists
	Given System Has access to the SFA.DAS.AssessmentOrgs.Api	
	 When I Create an Organisation
	| EndPointAssessorName | EndPointAssessorOrganisationId | EndPointAssessorUKPRN |
	| Test                 | 99999988                      | 10033333              |
    When I Create an Organisation
	| EndPointAssessorName | EndPointAssessorOrganisationId | EndPointAssessorUKPRN |
	| Test                 | 99999988                       | 10033333              |
	Then the response http status should be Bad Request
	And the response message should contain Organisation Has Already Been Created

Scenario: Update an Organisation Succesfully
	Given System Has access to the SFA.DAS.AssessmentOrgs.Api	
	When I Update an Organisation
	| EndPointAssessorName | EndPointAssessorOrganisationId | EndPointAssessorUKPRN |
	| Test Name            | 99999999                       | 10033670              |
	Then the response http status should be No Content
	And the Update should have occured
                
Scenario: Update an Organisation That does Not Exist
	Given System Has access to the SFA.DAS.AssessmentOrgs.Api	
	When I Update an Organisation With invalid Id
	| EndPointAssessorName | EndPointAssessorOrganisationId | EndPointAssessorUKPRN |
	| Test Name            | 99999999                       | 10005333              |
	Then the response http status should be Bad Request
	
Scenario: Update an Organisation with invalid PrimaryContact
	Given System Has access to the SFA.DAS.AssessmentOrgs.Api	
	When I Update an Organisation With Invalid Primary Contact
	| EndPointAssessorName | EndPointAssessorOrganisationId | EndPointAssessorUKPRN |
	| Test Name            | 99999999                       | 14              |
	Then the response http status should be Bad Request

Scenario: Update an Organisation with valid PrimaryContact
	Given System Has access to the SFA.DAS.AssessmentOrgs.Api	
	When I Update an Organisation With valid Primary Contact
	| EndPointAssessorName | EndPointAssessorOrganisationId | EndPointAssessorUKPRN |
	| Test Name            | 1234                       | 10033670              |
	Then the response http status should be No Content
	And the Organisation Status should be persisted as Live

Scenario: Delete an Organisation 
	Given System Has access to the SFA.DAS.AssessmentOrgs.Api	
	When I Delete an Organisation
	| EndPointAssessorName | EndPointAssessorOrganisationId | EndPointAssessorUKPRN |
	| Test                 | 99999777                       | 10033444              |
	Then the response http status should be No Content
	And the Organisation should be deleted

Scenario: Repeat Deleting an Organisation 
	Given System Has access to the SFA.DAS.AssessmentOrgs.Api	
	When I Delete an Organisation Twice
	| EndPointAssessorName | EndPointAssessorOrganisationId | EndPointAssessorUKPRN |
	| Test                 | 99999778                       | 10033444              |
	Then the response http status should be No Content
	And the Organisation should be deleted
	
