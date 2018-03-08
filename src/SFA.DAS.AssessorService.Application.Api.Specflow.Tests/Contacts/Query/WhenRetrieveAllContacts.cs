﻿using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Query.Services;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Query
{
    [Binding]
    public class WhenRetrieveAllContacts
    {
        private RestClientResult _restClientResult;
        private readonly OrganisationQueryService _organisationQueryService;
        private readonly ContactQueryService _contactQueryService;
        private List<OrganisationResponse> _organisationQueryViewModels = new List<OrganisationResponse>();
        private List<ContactResponse> _contactResponses = new List<ContactResponse>();

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
            var organisation = _restClientResult.Deserialise<OrganisationResponse>();

            _restClientResult =
                _contactQueryService.SearchForContactByOrganisationId(organisation.EndPointAssessorOrganisationId);

            _contactResponses = _restClientResult.Deserialise<List<ContactResponse>>().ToList();
        }

        [Then(@"the API returns all Contacts for an Organisation")]
        public void ThenTheAPIReturnsAllContactsForAnOrganisation()
        {
            _contactResponses.Count.Should().BeGreaterOrEqualTo(1);
        }
    }
}
