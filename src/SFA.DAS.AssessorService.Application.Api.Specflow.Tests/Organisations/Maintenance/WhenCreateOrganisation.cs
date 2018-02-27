namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations
{
    using FluentAssertions;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using TechTalk.SpecFlow;
    using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions;
    using System.Data;
    using Dapper;
    using System.Linq;
    using System.Collections.Generic;
    using System;
    using SFA.DAS.AssessorService.Domain.Enums;
    using System.Net.Http;
    using Newtonsoft.Json;

    [Binding]
    public sealed class WhenCreateOrganisation
    {
        private RestClient _restClient;
        private readonly IDbConnection _dbconnection;
        private Organisation _organisationRetrieved;
        private dynamic _organisationArguments;

        public WhenCreateOrganisation(RestClient restClient,
            IDbConnection dbconnection)
        {
            _restClient = restClient;
            _dbconnection = dbconnection;
        }

        [When(@"I Create an Organisation")]
        public void WhenICreateAnOrganisation(IEnumerable<dynamic> organisations)
        {
            _organisationArguments = organisations.First();

            var organisation = new CreateOrganisationRequest
            {
                EndPointAssessorName = _organisationArguments.EndPointAssessorName,
                EndPointAssessorOrganisationId = _organisationArguments.EndPointAssessorOrganisationId.ToString(),
                EndPointAssessorUKPRN = Convert.ToInt32(_organisationArguments.EndPointAssessorUKPRN),
                PrimaryContactId = null
            };

            _restClient.HttpResponseMessage = _restClient.HttpClient.PostAsJsonAsync(
                 "api/v1/organisations", organisation).Result;
            _restClient.Result = _restClient.HttpResponseMessage.Content.ReadAsStringAsync().Result;
        }


        [When(@"I Create an Organisation With Existing Primary Contact")]
        public void WhenICreateAnOrganisationWithExistingPrimaryContact(IEnumerable<dynamic> organisations)
        {
            _organisationArguments = organisations.First();

            HttpResponseMessage contactResponse = _restClient.HttpClient.GetAsync(
              "api/v1/contacts/user/John Coxhead").Result;
            var contactResult = contactResponse.Content.ReadAsStringAsync().Result;

            var contact = JsonConvert.DeserializeObject<Contact>(contactResult);

            var organisation = new CreateOrganisationRequest
            {
                EndPointAssessorName = _organisationArguments.EndPointAssessorName,
                EndPointAssessorOrganisationId = _organisationArguments.EndPointAssessorOrganisationId.ToString(),
                EndPointAssessorUKPRN = Convert.ToInt32(_organisationArguments.EndPointAssessorUKPRN),
                PrimaryContactId = contact.Id
            };

            _restClient.HttpResponseMessage = _restClient.HttpClient.PostAsJsonAsync(
                 "api/v1/organisations", organisation).Result;
            _restClient.Result = _restClient.HttpResponseMessage.Content.ReadAsStringAsync().Result;
        }

        [Then(@"the Organisation should be created")]
        public void ThenTheOrganisationShouldBeCreated()
        {
            var organisationsCreated = _dbconnection.Query<Organisation>
              ($"Select EndPointAssessorOrganisationId, EndPointAssessorUKPRN, EndPointAssessorName, OrganisationStatus From Organisations where EndPointAssessorOrganisationId = {_organisationArguments.EndPointAssessorOrganisationId}").ToList();
            _organisationRetrieved = organisationsCreated.First();

            organisationsCreated.Count.Should().Equals(1);

            _organisationRetrieved.EndPointAssessorOrganisationId.Should().Equals(_organisationArguments.EndPointAssessorOrganisationId);
            _organisationRetrieved.EndPointAssessorUKPRN.Should().Equals(_organisationArguments.EndPointAssessorUKPRN);
            _organisationRetrieved.EndPointAssessorName.Should().Equals(_organisationArguments.EndPointAssessorName);
        }

        [Then(@"the Organisation Status should be set to (.*)")]
        public void ThenTheOrganisationStatusShouldBeSetTo(string p0)
        {
            if (p0 == "Live")
            {
                _organisationRetrieved.OrganisationStatus.Should().Be(OrganisationStatus.Live);
            }
            else if (p0 == "New")
            {
                _organisationRetrieved.OrganisationStatus.Should().Be(OrganisationStatus.New);
            }
            else
            {
                throw new ApplicationException("Uknown OrganisationStatus");
            }
        }
    }
}
