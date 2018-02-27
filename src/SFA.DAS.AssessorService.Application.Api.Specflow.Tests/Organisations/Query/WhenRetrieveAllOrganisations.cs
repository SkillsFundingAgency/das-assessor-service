namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations
{
    using FluentAssertions;
    using Newtonsoft.Json;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System.Collections.Generic;
    using System.Net.Http;
    using TechTalk.SpecFlow;

    [Binding]
    public class WhenRetrieveAllOrganisations
    {
        private readonly RestClient _restClient;
        private List<Organisation> _organisationQueryViewModels = new List<Organisation>();

        public WhenRetrieveAllOrganisations(RestClient restClient)
        {
            _restClient = restClient;
        }

        [When(@"I Request All Organisations to be retrieved")]
        public void WhenIRequestAllOrganisationsToBeRetrieved()
        {
            HttpResponseMessage response =  _restClient.HttpClient.GetAsync(
               "api/v1/organisations").Result;
            if (response.IsSuccessStatusCode)
            {
                _restClient.Result = response.Content.ReadAsStringAsync().Result;
                _restClient.HttpResponseMessage = response;

                _organisationQueryViewModels = JsonConvert.DeserializeObject<List<Organisation>>(_restClient.Result);
            }
        }

        [Then(@"the API returns all Organisations")]
        public void ThenTheAPIReturnsAllOrganisations()
        {
            _organisationQueryViewModels.Count.Should().BeGreaterOrEqualTo(1);
        }
    }
}
