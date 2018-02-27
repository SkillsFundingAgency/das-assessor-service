namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations
{
    using FluentAssertions;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using TechTalk.SpecFlow;

    [Binding]
    public class WhenSearchOrganisationByUkprn
    {
        private Organisation _organisationQueryViewModel = new Organisation();
        private readonly RestClient _restClient;

        public WhenSearchOrganisationByUkprn(RestClient restClient)
        {
            _restClient = restClient;
        }

        [When(@"I search for an organisation with its ukprn set to (.*)")]
        public void WhenISearchForAnOrganisationWithItsUkprnSetTo(int p0)
        {
            int ukprn = p0;

            _restClient.HttpResponseMessage = _restClient.HttpClient.GetAsync(
                  $"api/v1/organisations/{ukprn}").Result;
            _restClient.Result = _restClient.HttpResponseMessage.Content.ReadAsStringAsync().Result;
        }

        [Then(@"the API returns an appropriate Organisation")]
        public void ThenTheAPIReturnsAnAppropriateOrganisation()
        {
            _organisationQueryViewModel.Should().NotBeNull();
        }
    }
}

