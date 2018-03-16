using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.DatabaseUtils;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Query
{
    [Binding]
    public class WhenRetrieveAllContacts : WhenRetrieveContactBase
    {
        private RestClientResult _restClientResult;       
        private readonly ContactQueryService _contactQueryService;        
        private List<ContactResponse> _contactResponses = new List<ContactResponse>();

        public WhenRetrieveAllContacts(RestClientResult restClientResult,
            ContactData contactData,
            OrganisationData organisationData,         
            ContactQueryService contactQueryService) : base(contactData, organisationData)       
        {
            _restClientResult = restClientResult;          
            _contactQueryService = contactQueryService;
        }
             
        [When(@"I Request All Contacts to be retrieved BY Organisation with Id (.*)")]
        public void WhenIRequestAllContactsToBeRetrievedByOrganisationWithId(string p0)
        { 
            Setup(p0, "dummyUser", "dummyEmail@hotmail.com");
            
            _restClientResult =
                _contactQueryService.SearchForContactByOrganisationId(p0);

            _contactResponses = _restClientResult.Deserialise<List<ContactResponse>>().ToList();
        }

        [Then(@"the API returns all Contacts for an Organisation")]
        public void ThenTheAPIReturnsAllContactsForAnOrganisation()
        {
            _contactResponses.Count.Should().BeGreaterOrEqualTo(1);
        }
    }
}
