using FizzWare.NBuilder;
using FluentAssertions;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.DatabaseUtils;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Query.Services;
using SFA.DAS.AssessorService.Domain.Entities;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Query
{
    [Binding]
    public class WhenSearchOrganisationByUkprn
    {
        private readonly OrganisationData _organisationData;
        private readonly OrganisationQueryService _organisationQueryService;
        private OrganisationResponse _organisationResponse = new OrganisationResponse();
       
        public WhenSearchOrganisationByUkprn(OrganisationQueryService organisationQueryService,
            OrganisationData organisationData)
        {
            _organisationQueryService = organisationQueryService;
            _organisationData = organisationData;
        }

        [When(@"I search for an organisation with its ukprn set to (.*)")]
        public void WhenISearchForAnOrganisationWithItsUkprnSetTo(int p0)
        {
            var ukprn = p0;

            CreateOrganisationToQueryAgainst(p0);

            var result = _organisationQueryService.SearchOrganisationByUkPrn(ukprn);
            _organisationResponse = result.Deserialise<OrganisationResponse>();
        }       

        [When(@"I search for an organisation with its ukprn set to invalid organisation (.*)")]
        public void WhenISearchForAnOrganisationWithItsUkprnSetToInvalidOrganisation(int p0)
        {
            var ukprn = p0;

            CreateOrganisationToQueryAgainst(p0 + 1);

            var result = _organisationQueryService.SearchOrganisationByUkPrn(ukprn);
            _organisationResponse = result.Deserialise<OrganisationResponse>();
        }

        [Then(@"the API returns an appropriate Organisation")]
        public void ThenTheAPIReturnsAnAppropriateOrganisation()
        {
            _organisationResponse.Should().NotBeNull();
        }

        private void CreateOrganisationToQueryAgainst(int p0)
        {
            var organisation = Builder<Organisation>.CreateNew()
                .With(q => q.EndPointAssessorOrganisationId = "999999")
                .With(q => q.EndPointAssessorUkprn = p0)
                .Build();

            _organisationData.Insert(organisation);
        }
    }
}

