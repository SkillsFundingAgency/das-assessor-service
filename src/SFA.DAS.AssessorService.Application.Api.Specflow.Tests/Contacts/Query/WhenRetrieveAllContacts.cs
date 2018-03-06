using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Query;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Query.Services;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations
{
    using FluentAssertions;
    using Newtonsoft.Json;
    using SFA.DAS.AssessorService.Api.Types;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using AssessorService.Api.Types.Models;
    using TechTalk.SpecFlow;

    [Binding]
    public class WhenRetrieveAllContacts
    {
        private RestClientResult _restClientResult;
        private readonly OrganisationQueryService _organisationQueryService;
        private readonly ContactQueryService _contactQueryService;
        private List<Organisation> _organisationQueryViewModels = new List<Organisation>();
        private List<Contact> _contacts = new List<Contact>();

        public WhenRetrieveAllContacts(RestClientResult restClientResult,
            OrganisationQueryService organisationQueryService,
            ContactQueryService contactQueryService)
        {
            _restClientResult = restClientResult;
            _organisationQueryService = organisationQueryService;
            _contactQueryService = contactQueryService;
        }


        [When(@"I Request All Contacts to be retrieved BY Organisation")]
        public void WhenIRequestAllContactsToBeRetrievedBYOrganisation()
        {
            _restClientResult = _organisationQueryService.SearchOrganisationByUkPrn(10000000);
            var organisation = _restClientResult.Deserialise<Organisation>();

            _restClientResult =
                _contactQueryService.SearchForContactByOrganisationId(organisation.EndPointAssessorOrganisationId);

            _contacts = _restClientResult.Deserialise<List<Contact>>().ToList();
        }


        [Then(@"the API returns all Contacts for an Organisation")]
        public void ThenTheAPIReturnsAllContactsForAnOrganisation()
        {
            _contacts.Count.Should().BeGreaterOrEqualTo(1);
        }
    }
}
