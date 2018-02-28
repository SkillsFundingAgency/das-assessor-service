namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations
{
    using FluentAssertions;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using AssessorService.Api.Types.Models;
    using TechTalk.SpecFlow;

    [Binding]
    public class WhenRetrieveContactByUserName
    {
        private readonly RestClient _restClient;
        private Contact _contactQueryViewModel;

        private dynamic _contactArgument;

        public WhenRetrieveContactByUserName(RestClient restClient)
        {
            _restClient = restClient;
        }

        [When(@"I Request Contacts to be retrieved By Username")]
        public void WhenIRequestContactsToBeRetrievedByUsername(IEnumerable<dynamic> contacts)
        {
            _contactArgument = contacts.First();
            var userName = _contactArgument.username;

            HttpResponseMessage response = _restClient.HttpClient.GetAsync(
                        $"api/v1/contacts/user/{userName}").Result;

            _restClient.Result = response.Content.ReadAsStringAsync().Result;
            _restClient.HttpResponseMessage = response;

            _contactQueryViewModel = JsonConvert.DeserializeObject<Contact>(_restClient.Result);
        }

        [Then(@"the API returns valid Contact")]
        public void ThenTheAPIReturnsValidContact()
        {
            _contactQueryViewModel.ContactName.Should().Be(_contactArgument.username);
            _contactQueryViewModel.ContactEmail.Should().Be(_contactArgument.emailaddress);
        }
    }
}
