using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using FluentAssertions;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Query;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Helpers;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Maintenance.Services;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Query.Services;
using SFA.DAS.AssessorService.Domain.Consts;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Maintenance
{
    [Binding]
    public sealed class WhenUpdateOrganisation
    {
        private RestClientResult _restClient;
        private readonly OrganisationQueryService _organisationQueryService;
        private readonly OrganisationService _organisationService;
        private readonly ContactQueryService _contactQueryService;
        private readonly UpdateOrganisationRequestBuilder _updateOrganisationRequestBuilder;
        private readonly IDbConnection _dbconnection;
        private Organisation _organisationRetrieved;
        private dynamic _organisationArguments;

        public WhenUpdateOrganisation(RestClientResult restClient,
            OrganisationQueryService organisationQueryService,
            OrganisationService organisationService,
            ContactQueryService contactQueryService,
            UpdateOrganisationRequestBuilder updateOrganisationRequestBuilder,
            IDbConnection dbconnection)
        {
            _restClient = restClient;
            _organisationQueryService = organisationQueryService;
            _organisationService = organisationService;
            _contactQueryService = contactQueryService;
            _updateOrganisationRequestBuilder = updateOrganisationRequestBuilder;
            _dbconnection = dbconnection;
        }

        [When(@"I Update an Organisation")]
        public void WhenIUpdateAnOrganisation(IEnumerable<dynamic> organisations)
        {
            _organisationArguments = organisations.First();

            var restClient = _organisationQueryService.SearchOrganisationByUkPrn(10000000);
            var organisation = restClient.Deserialise<Organisation>();
            organisation.EndPointAssessorName = _organisationArguments.EndPointAssessorName;

            var updateOrganisationRequest = _updateOrganisationRequestBuilder.Build(organisation);

            _organisationService.PutOrganisation(updateOrganisationRequest);
        }


        [When(@"I Update an Organisation With invalid Id")]
        public void WhenIUpdateAnOrganisationWithInvalidId(IEnumerable<dynamic> organisations)
        {
            var organisation = new UpdateOrganisationRequest
            {
                EndPointAssessorOrganisationId = "9999999999",
                PrimaryContact = null,
                EndPointAssessorName = "XXX"
            };

            _restClient = _organisationService.PutOrganisation(organisation);
        }

        [When(@"I Update an Organisation With Invalid Primary Contact")]
        public void WhenIUpdateAnOrganisationWithInvalidPrimaryContact(IEnumerable<dynamic> organisations)
        {
            _organisationArguments = organisations.First();
            var restClient = _organisationQueryService.SearchOrganisationByUkPrn(10000000);
            var organisation = restClient.Deserialise<Organisation>();

            var updateOrganisationRequest = _updateOrganisationRequestBuilder.Build(organisation);
            updateOrganisationRequest.PrimaryContact = "12323";

            _organisationService.PutOrganisation(updateOrganisationRequest);
        }


        [When(@"I Update an Organisation With valid Primary Contact")]
        public void WhenIUpdateAnOrganisationWithValidPrimaryContact(IEnumerable<dynamic> organisations)
        {
            _organisationArguments = organisations.First();

            var contactResult = _contactQueryService.SearchForContactByUserName("jcoxhead");
            var contact = contactResult.Deserialise<Contact>();

            var restClient = _organisationQueryService.SearchOrganisationByUkPrn(10000000);
            var organisation = restClient.Deserialise<Organisation>();
            organisation.PrimaryContact = contact.Username;

            var updateOrganisationRequest = _updateOrganisationRequestBuilder.Build(organisation);
            _organisationService.PutOrganisation(updateOrganisationRequest);
        }

        [Then(@"the Update should have occured")]
        public void ThenTheUpdateShouldHaveOccured()
        {
            var organisations
                = _dbconnection.Query<Organisation>
                    ($"Select EndPointAssessorOrganisationId, EndPointAssessorUKPRN, EndPointAssessorName, Status From Organisations where EndPointAssessorUKPRN = {_organisationArguments.EndPointAssessorUKPRN}").ToList();
            _organisationRetrieved = organisations.First();

            organisations.Count.Should().Equals(1);

            _organisationRetrieved.EndPointAssessorName.Should().Be(_organisationArguments.EndPointAssessorName);
        }

        [Then(@"the Organisation Status should be persisted as Live")]
        public void ThenTheOrganisationStatusShouldBePersistedAsLive()
        {
            var organisationUpdated = _dbconnection.Query<Organisation>
              ($"Select EndPointAssessorOrganisationId, EndPointAssessorUKPRN, EndPointAssessorName, Status From Organisations where EndPointAssessorOrganisationId = {_organisationArguments.EndPointAssessorOrganisationId}").ToList();
            _organisationRetrieved = organisationUpdated.First();

            _organisationRetrieved.Status.Should().Be(OrganisationStatus.Live);
        }
    }
}
