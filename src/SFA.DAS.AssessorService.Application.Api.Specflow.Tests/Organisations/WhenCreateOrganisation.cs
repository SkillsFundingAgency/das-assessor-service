namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations
{
    using FluentAssertions;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using TechTalk.SpecFlow;
    using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions;
    using System.Data;
    using System.Data.SqlClient;
    using Dapper;
    using SFA.DAS.AssessorService.Domain.Entities;
    using System.Linq;

    [Binding]
    public sealed class WhenCreateOrganisation
    {
        private RestClient _restClient;
        private readonly IDbConnection _dbconnection;

        public WhenCreateOrganisation(RestClient restClient,
            IDbConnection dbconnection)
        {
            _restClient = restClient;
            _dbconnection = dbconnection;
        }

        [When(@"I Create an Organisation")]
        public void WhenICreateAnOrganisation()
        {
            var organisation = new OrganisationCreateViewModel
            {
                EndPointAssessorName = "Test",
                EndPointAssessorOrganisationId = "99999999",
                EndPointAssessorUKPRN = 10033333,
                PrimaryContactId = null
            };

            _restClient.HttpResponseMessage = _restClient.HttpClient.PostAsJsonAsync(
                 "api/v1/organisations", organisation).Result;
        }

        [Then(@"the Location Header should be set")]
        public void ThenTheLocationHeaderShouldBeSet()
        {
            _restClient.HttpResponseMessage.Headers.Location.Should().NotBeNull();
        }

        [Then(@"the Organisation should be created")]
        public void ThenTheOrganisationShouldBeCreated()
        {
           var xx =  _dbconnection.Query<Organisation>
             ("Select * From Organisations").ToList();
        }
    }
}
