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
    public class WhenSearchContactsByUserName
    {
        private readonly RestClientResult _restClient;
        private Contact _contactQueryViewModel;

        private dynamic _contactArgument;

        public WhenSearchContactsByUserName(RestClientResult restClient)
        {
            _restClient = restClient;
        }

        [When(@"Client Searches Contacts By Username")]
        public void WhenClientSearchesContactsByUsername(IEnumerable<dynamic> contacts)
        {
            _contactArgument = contacts.First();
            var userName = _contactArgument.username;

            HttpResponseMessage response = _restClient.HttpClient.GetAsync(
                        $"api/v1/contacts/user/{userName}").Result;

            _restClient.JsonResult = response.Content.ReadAsStringAsync().Result;
            _restClient.HttpResponseMessage = response;

            _contactQueryViewModel = JsonConvert.DeserializeObject<Contact>(_restClient.JsonResult);
        }

        [Then(@"the API returns a valid Contact")]
        public void ThenTheAPIReturnsaValidContact()
        {
            _contactQueryViewModel.Username.Should().Be(_contactArgument.username);
            _contactQueryViewModel.Email.Should().Be(_contactArgument.emailaddress);
        }
    }
}
