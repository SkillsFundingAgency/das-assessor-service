namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations
{
    using FluentAssertions;
    using Newtonsoft.Json;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using TechTalk.SpecFlow;

    [Binding]
    public class WhenRetrieveContactByUserNameAndEmailAddress
    {
        private readonly RestClient _restClient;
        private ContactQueryViewModel _contactQueryViewModel;

        private dynamic _contactArgument;

        public WhenRetrieveContactByUserNameAndEmailAddress(RestClient restClient)
        {
            _restClient = restClient;
        }

        [When(@"I Request Contacts to be retrieved By Username and Email Address")]
        public void WhenIRequestContactsToBeRetrievedByUsernameAndEmailAddress(IEnumerable<dynamic> contacts)
        {
            _contactArgument = contacts.First();
            var userName = _contactArgument.username;
            var emailAddress = _contactArgument.emailaddress;

            HttpResponseMessage response = _restClient.HttpClient.GetAsync(
                        $"api/v1/contacts/user/{userName}/{emailAddress}").Result;

            _restClient.Result = response.Content.ReadAsStringAsync().Result;
            _restClient.HttpResponseMessage = response;

            _contactQueryViewModel = JsonConvert.DeserializeObject<ContactQueryViewModel>(_restClient.Result);
        }

        [Then(@"the API returns valid Contact")]
        public void ThenTheAPIReturnsValidContact()
        {
            _contactQueryViewModel.ContactName.Should().Be(_contactArgument.username);
            _contactQueryViewModel.ContactEmail.Should().Be(_contactArgument.emailaddress);
        }
    }
}
