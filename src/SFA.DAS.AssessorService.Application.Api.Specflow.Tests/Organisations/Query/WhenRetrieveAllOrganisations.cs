using System.Collections.Generic;
using FizzWare.NBuilder;
using FluentAssertions;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Query.Services;
using SFA.DAS.AssessorService.Domain.Entities;
using TechTalk.SpecFlow;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.DatabaseUtils;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Query
{
    [Binding]
    public class WhenRetrieveAllOrganisations
    {
        private readonly OrganisationQueryService _organisationQueryService;
        private readonly OrganisationData _organisationData;      
        private List<OrganisationResponse> _organisationResponses = new List<OrganisationResponse>();      

        public WhenRetrieveAllOrganisations(OrganisationQueryService organisationQueryService,
            OrganisationData organisationData)
        {
            _organisationQueryService = organisationQueryService;
            _organisationData = organisationData;          
        }

        [Scope(Scenario = "Retrieve All Organisations")]
        [BeforeScenario()]
        public void Setup()
        {          
            var organisation = Builder<Organisation>.CreateNew()
                .With(q => q.EndPointAssessorOrganisationId = "999999")
                .Build();

            var result = _organisationData.Insert(organisation);
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
