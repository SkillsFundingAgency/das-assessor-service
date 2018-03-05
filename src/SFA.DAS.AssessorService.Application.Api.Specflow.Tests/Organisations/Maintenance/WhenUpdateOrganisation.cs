namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations
{
    using FluentAssertions;
    using TechTalk.SpecFlow;
    using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions;
    using System.Data;
    using Dapper;
    using System.Linq;
    using System.Collections.Generic;
    using System.Net.Http;
    using Newtonsoft.Json;
    using System;
    using AssessorService.Api.Types.Models;
    using Domain.Consts;
    using SFA.DAS.AssessorService.Api.Types;

    [Binding]
    public sealed class WhenUpdateOrganisation
    {
        private RestClient _restClient;
        private readonly IDbConnection _dbconnection;
        private Organisation _organisationRetrieved;
        private dynamic _organisationArguments;

        public WhenUpdateOrganisation(RestClient restClient,
            IDbConnection dbconnection)
        {
            _restClient = restClient;
            _dbconnection = dbconnection;
        }

        [When(@"I Update an Organisation")]
        public void WhenIUpdateAnOrganisation(IEnumerable<dynamic> organisations)
        {
            _organisationArguments = organisations.First();

            HttpResponseMessage organisationResponse = _restClient.HttpClient.GetAsync(
            "api/v1/organisations/10000000").Result;
            var organisationResult = organisationResponse.Content.ReadAsStringAsync().Result;
            var organisationQueryViewModel = JsonConvert.DeserializeObject<Organisation>(organisationResult);

            var organisation = new UpdateOrganisationRequest
            {
                EndPointAssessorOrganisationId = organisationQueryViewModel.EndPointAssessorOrganisationId,
                PrimaryContact = organisationQueryViewModel.PrimaryContact,
                EndPointAssessorName = _organisationArguments.EndPointAssessorName,
            };

            _restClient.HttpResponseMessage = _restClient.HttpClient.PutAsJsonAsync(
                 "api/v1/organisations", organisation).Result;
            _restClient.Result = _restClient.HttpResponseMessage.Content.ReadAsStringAsync().Result;
        }

        [Then(@"the Update should have occured")]
        public void ThenTheUpdateShouldHaveOccured()
        {
            var organisationsCreated = _dbconnection.Query<Organisation>
              ($"Select EndPointAssessorOrganisationId, EndPointAssessorUKPRN, EndPointAssessorName, Status From Organisations where EndPointAssessorUKPRN = {_organisationArguments.EndPointAssessorUKPRN}").ToList();
            _organisationRetrieved = organisationsCreated.First();

            organisationsCreated.Count.Should().Equals(1);

            _organisationRetrieved.EndPointAssessorName.Should().Be(_organisationArguments.EndPointAssessorName);
        }


        [When(@"I Update an Organisation With invalid Id")]
        public void WhenIUpdateAnOrganisationWithInvalidId(IEnumerable<dynamic> organisations)
        {
            var organisation = new UpdateOrganisationRequest
            {
                EndPointAssessorOrganisationId ="9999999999",
                PrimaryContact = null,
                EndPointAssessorName = "XXX"
            };

            _restClient.HttpResponseMessage = _restClient.HttpClient.PutAsJsonAsync(
                 "api/v1/organisations", organisation).Result;
            _restClient.Result = _restClient.HttpResponseMessage.Content.ReadAsStringAsync().Result;
        }

        [When(@"I Update an Organisation With Invalid Primary Contact")]
        public void WhenIUpdateAnOrganisationWithInvalidPrimaryContact(IEnumerable<dynamic> organisations)
        {
            _organisationArguments = organisations.First();

            HttpResponseMessage organisationResponse = _restClient.HttpClient.GetAsync(
            "api/v1/organisations/10000000").Result;
            var organisationResult = organisationResponse.Content.ReadAsStringAsync().Result;
            var organisationQueryViewModel = JsonConvert.DeserializeObject<Organisation>(organisationResult);

            var organisation = new UpdateOrganisationRequest
            {
                EndPointAssessorOrganisationId = organisationQueryViewModel.EndPointAssessorOrganisationId,
                PrimaryContact = "12323",
                EndPointAssessorName = _organisationArguments.EndPointAssessorName
            };

            _restClient.HttpResponseMessage = _restClient.HttpClient.PutAsJsonAsync(
                 "api/v1/organisations", organisation).Result;
            _restClient.Result = _restClient.HttpResponseMessage.Content.ReadAsStringAsync().Result;
        }


        [When(@"I Update an Organisation With valid Primary Contact")]
        public void WhenIUpdateAnOrganisationWithValidPrimaryContact(IEnumerable<dynamic> organisations)
        {
            _organisationArguments = organisations.First();

            HttpResponseMessage contactResponse = _restClient.HttpClient.GetAsync(
           "api/v1/contacts/user/jcoxhead").Result;
            var contactResult = contactResponse.Content.ReadAsStringAsync().Result;

            var contact = JsonConvert.DeserializeObject<Contact>(contactResult);

            HttpResponseMessage organisationResponse = _restClient.HttpClient.GetAsync(
            "api/v1/organisations/10000000").Result;
            var organisationResult = organisationResponse.Content.ReadAsStringAsync().Result;
            var organisationQueryViewModel = JsonConvert.DeserializeObject<Organisation>(organisationResult);

            var organisation = new UpdateOrganisationRequest
            {
                EndPointAssessorOrganisationId = organisationQueryViewModel.EndPointAssessorOrganisationId,
                PrimaryContact = contact.Username,
                EndPointAssessorName = _organisationArguments.EndPointAssessorName
            };

            _restClient.HttpResponseMessage = _restClient.HttpClient.PutAsJsonAsync(
                 "api/v1/organisations", organisation).Result;
            _restClient.Result = _restClient.HttpResponseMessage.Content.ReadAsStringAsync().Result;
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
