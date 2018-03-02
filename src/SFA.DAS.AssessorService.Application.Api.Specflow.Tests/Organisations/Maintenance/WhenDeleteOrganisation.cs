namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Maintenance
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using AssessorService.Api.Types.Models;
    using Dapper;
    using Domain.Consts;
    using Extensions;
    using FluentAssertions;
    using Newtonsoft.Json;
    using TechTalk.SpecFlow;

    [Binding]
    public sealed class WhenDeleteOrganisation
    {
        private readonly RestClient _restClient;
        private readonly IDbConnection _dbconnection;
        private Organisation _organisationRetrieved;
        private dynamic _organisationArguments;

        public WhenDeleteOrganisation(RestClient restClient,
            IDbConnection dbconnection)
        {
            _restClient = restClient;
            _dbconnection = dbconnection;
        }

        [When(@"I Delete an Organisation")]
        public void WhenIDeleteAnOrganisation(IEnumerable<dynamic> organisations)
        {
            _organisationArguments = organisations.First();

            var organisation = new CreateOrganisationRequest
            {
                EndPointAssessorName = _organisationArguments.EndPointAssessorName,
                EndPointAssessorOrganisationId = _organisationArguments.EndPointAssessorOrganisationId.ToString(),
                EndPointAssessorUkprn = Convert.ToInt32(_organisationArguments.EndPointAssessorUKPRN),
                PrimaryContact = null
            };

            _restClient.HttpResponseMessage = _restClient.HttpClient.PostAsJsonAsync(
                 "api/v1/organisations", organisation).Result;
            _restClient.Result = _restClient.HttpResponseMessage.Content.ReadAsStringAsync().Result;

            var organisationCreated = JsonConvert.DeserializeObject<Organisation>(_restClient.Result);

            _restClient.HttpResponseMessage = _restClient.HttpClient.DeleteAsJsonAsync($"api/v1/organisations?endPointAssessorOrganisationId={organisationCreated.EndPointAssessorOrganisationId}").Result;
        }


        [When(@"I Delete an Organisation Twice")]
        public void WhenIDeleteAnOrganisationTwice(IEnumerable<dynamic> organisations)
        {
            _organisationArguments = organisations.First();

            var organisation = new CreateOrganisationRequest
            {
                EndPointAssessorName = _organisationArguments.EndPointAssessorName,
                EndPointAssessorOrganisationId = _organisationArguments.EndPointAssessorOrganisationId.ToString(),
                EndPointAssessorUkprn = Convert.ToInt32(_organisationArguments.EndPointAssessorUKPRN),
                PrimaryContact = null
            };

            _restClient.HttpResponseMessage = _restClient.HttpClient.PostAsJsonAsync(
                 "api/v1/organisations", organisation).Result;
            _restClient.Result = _restClient.HttpResponseMessage.Content.ReadAsStringAsync().Result;

            var organisationCreated = JsonConvert.DeserializeObject<Organisation>(_restClient.Result);

            _restClient.HttpResponseMessage = _restClient.HttpClient.DeleteAsJsonAsync($"api/v1/organisations?endPointAssessorOrganisationId={organisationCreated.EndPointAssessorOrganisationId}").Result;
            _restClient.HttpResponseMessage = _restClient.HttpClient.DeleteAsJsonAsync($"api/v1/organisations?endPointAssessorOrganisationId={organisationCreated.EndPointAssessorOrganisationId}").Result;
        }

        [Then(@"the Organisation should be deleted")]
        public void ThenTheOrganisationShouldBeDeleted()
        {
            var organisationsCreated = _dbconnection.Query<Organisation>
            ($"Select EndPointAssessorOrganisationId, EndPointAssessorUKPRN, EndPointAssessorName, Status From Organisations where EndPointAssessorOrganisationId = {_organisationArguments.EndPointAssessorOrganisationId}").ToList();
            _organisationRetrieved = organisationsCreated.First();

            _organisationRetrieved.Status.Should().Be(OrganisationStatus.Deleted);
        }
    }
}
