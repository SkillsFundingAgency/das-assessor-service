using FluentAssertions;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.DatabaseUtils;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Query
{
    [Binding]
    public class WhenSearchContactsByUserName : WhenRetrieveContactBase
    {
        private readonly ContactQueryService _contactQueryService;

        private dynamic _contactArgument;
        private ContactResponse _contactResponse;
        private RestClientResult _restClientResult;

        public WhenSearchContactsByUserName(
            ContactData contactData,
            OrganisationData organisationData,
            ContactQueryService contactQueryService,
            RestClientResult restClientResult) : base(contactData, organisationData)
        {
            _contactQueryService = contactQueryService;
            _restClientResult = restClientResult;
        }

        [When(@"Client Searches Contacts By Username")]
        public void WhenClientSearchesContactsByUsername(IEnumerable<dynamic> contacts)
        {            
            _contactArgument = contacts.First();         
            string userName = _contactArgument.UserName;
            string emailAddress = _contactArgument.EmailAddress;
            string endPointAssessorOrganisationId = _contactArgument.EndPointAssessorOrganisationId.ToString();

            Setup(endPointAssessorOrganisationId, userName, emailAddress);

            _restClientResult = _contactQueryService.SearchForContactByUserName(userName);
            _contactResponse = _restClientResult.Deserialise<ContactResponse>();
        }       

        [Then(@"the API returns a valid Contact")]
        public void ThenTheAPIReturnsaValidContact()
        {
            _contactResponse.Username.Should().Be(_contactArgument.UserName);
            _contactResponse.Email.Should().Be(_contactArgument.EmailAddress);
        }
    }
}