using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Query
{
    [Binding]
    public class WhenSearchContactsByUserName
    {
        private readonly ContactQueryService _contactQueryService;

        private dynamic _contactArgument;
        private ContactResponse _contactResponse;
        private RestClientResult _restClientResult;

        public WhenSearchContactsByUserName(
            ContactQueryService contactQueryService,
            RestClientResult restClientResult)
        {
            _contactQueryService = contactQueryService;
            _restClientResult = restClientResult;
        }

        [When(@"Client Searches Contacts By UserName")]
        public void WhenClientSearchesContactsByUsername(IEnumerable<dynamic> contacts)
        {
            _contactArgument = contacts.First();
            var userName = _contactArgument.username;

            _restClientResult = _contactQueryService.SearchForContactByUserName(userName);

            _contactResponse = _restClientResult.Deserialise<ContactResponse>();
        }

        [Then(@"the API returns a valid Contact")]
        public void ThenTheAPIReturnsaValidContact()
        {
            _contactResponse.Username.Should().Be(_contactArgument.username);
            _contactResponse.Email.Should().Be(_contactArgument.emailaddress);
        }
    }
}