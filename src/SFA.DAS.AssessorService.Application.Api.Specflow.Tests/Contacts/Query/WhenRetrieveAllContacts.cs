namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations
{
    using FluentAssertions;
    using Newtonsoft.Json;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using TechTalk.SpecFlow;

    [Binding]
    public class WhenRetrieveAllContacts
    {
        private readonly RestClient _restClient;
        private List<OrganisationQueryViewModel> _organisationQueryViewModels = new List<OrganisationQueryViewModel>();
        private List<ContactQueryViewModel> _contactQueryViewModels = new List<ContactQueryViewModel>();

        public WhenRetrieveAllContacts(RestClient restClient)
        {
            _restClient = restClient;
        }


        [When(@"I Request All Contacts to be retrieved BY Organisation")]
        public void WhenIRequestAllContactsToBeRetrievedBYOrganisation()
        {
            HttpResponseMessage response = _restClient.HttpClient.GetAsync(
               "api/v1/organisations").Result;
            if (response.IsSuccessStatusCode)
            {
                _restClient.Result = response.Content.ReadAsStringAsync().Result;
                _restClient.HttpResponseMessage = response;

                _organisationQueryViewModels = JsonConvert.DeserializeObject<List<OrganisationQueryViewModel>>(_restClient.Result);

                var organisation = _organisationQueryViewModels.First(q => q.EndPointAssessorUKPRN == 10000000);

                response = _restClient.HttpClient.GetAsync(
                        $"api/v1/contacts/{organisation.Id}").Result;


                _contactQueryViewModels = JsonConvert.DeserializeObject<List<ContactQueryViewModel>>(_restClient.Result);

                _restClient.HttpResponseMessage = response;
            }
            else
            {
                throw new ApplicationException("Cannot find any organisations in database");
            }
        }


        [When(@"I Request All Contacts to be retrieved By an Invalid Organisation")]
        public void WhenIRequestAllContactsToBeRetrievedByAnInvalidOrganisation()
        {
            var id = Guid.NewGuid();

            HttpResponseMessage response = _restClient.HttpClient.GetAsync(
                        $"api/v1/contacts/{id}").Result;

            _restClient.HttpResponseMessage = response;
        }

        [Then(@"the API returns all Contacts for an Organisation")]
        public void ThenTheAPIReturnsAllContactsForAnOrganisation()
        {
            _contactQueryViewModels.Count.Should().BeGreaterOrEqualTo(1);
        }
    }
}
