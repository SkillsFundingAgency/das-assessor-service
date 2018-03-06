using FluentAssertions;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Query.Services;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Query
{
    [Binding]
    public class WhenSearchOrganisationByUkprn
    {
        private readonly OrganisationQueryService _organisationQueryService;
        private Organisation _organisation = new Organisation();
    
        public WhenSearchOrganisationByUkprn(OrganisationQueryService organisationQueryService)
        {
            _organisationQueryService = organisationQueryService;
        }

        [When(@"I search for an organisation with its ukprn set to (.*)")]
        public void WhenISearchForAnOrganisationWithItsUkprnSetTo(int p0)
        {
            var ukprn = p0;

            var result = _organisationQueryService.SearchOrganisationByUkPrn(ukprn);
            _organisation = result.Deserialise<Organisation>();
        }

        [Then(@"the API returns an appropriate Organisation")]
        public void ThenTheAPIReturnsAnAppropriateOrganisation()
        {
            _organisation.Should().NotBeNull();
        }
    }
}

