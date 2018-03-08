using System.Collections.Generic;
using FluentAssertions;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Query.Services;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Query
{
    [Binding]
    public class WhenRetrieveAllOrganisations
    {
        private readonly OrganisationQueryService _organisationQueryService;
        private List<OrganisationResponse> _organisationResponses = new List<OrganisationResponse>();

        public WhenRetrieveAllOrganisations(OrganisationQueryService organisationQueryService)
        {
            _organisationQueryService = organisationQueryService;
        }

        [When(@"I Request All Organisations to be retrieved")]
        public void WhenIRequestAllOrganisationsToBeRetrieved()
        {
            var result = _organisationQueryService.GetOrganisations();
            _organisationResponses = result.Deserialise<List<OrganisationResponse>>();
        }

        [Then(@"the API returns all Organisations")]
        public void ThenTheApiReturnsAllOrganisations()
        {
            _organisationResponses.Count.Should().BeGreaterOrEqualTo(1);
        }
    }
}
