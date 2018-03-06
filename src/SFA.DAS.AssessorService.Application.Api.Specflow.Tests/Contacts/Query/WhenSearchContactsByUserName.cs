using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Query;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions;

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
        private readonly ContactQueryService _contactQueryService;
        private RestClientResult _restClientResult;
        private Contact _contactQueryViewModel;

        private dynamic _contactArgument;

        public WhenSearchContactsByUserName(
            ContactQueryService contactQueryService,
            RestClientResult restClientResult)
        {
            _contactQueryService = contactQueryService;
            _restClientResult = restClientResult;
        }

        [When(@"Client Searches Contacts By Username")]
        public void WhenClientSearchesContactsByUsername(IEnumerable<dynamic> contacts)
        {
            _contactArgument = contacts.First();
            var userName = _contactArgument.username;

            _restClientResult = _contactQueryService.SearchForContactByUserName(userName);

            _contactQueryViewModel = _restClientResult.Deserialise<Contact>();
        }

        [Then(@"the API returns a valid Contact")]
        public void ThenTheAPIReturnsaValidContact()
        {
            _contactQueryViewModel.Username.Should().Be(_contactArgument.username);
            _contactQueryViewModel.Email.Should().Be(_contactArgument.emailaddress);
        }
    }
}
