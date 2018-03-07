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
        private List<Organisation> _organisations = new List<Organisation>();

        public WhenRetrieveAllOrganisations(OrganisationQueryService organisationQueryService)
        {
            _organisationQueryService = organisationQueryService;
        }

        [When(@"I Request All Organisations to be retrieved")]
        public void WhenIRequestAllOrganisationsToBeRetrieved()
        {
            var result = _organisationQueryService.GetOrganisations();
            _organisations = result.Deserialise<List<Organisation>>();
        }

        [Then(@"the API returns all Organisations")]
        public void ThenTheApiReturnsAllOrganisations()
        {
            _organisations.Count.Should().BeGreaterOrEqualTo(1);
        }
    }
}
