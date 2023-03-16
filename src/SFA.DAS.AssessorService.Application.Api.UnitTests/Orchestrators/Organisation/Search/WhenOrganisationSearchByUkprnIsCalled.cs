using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Orchestrators;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Orchestrators.OrganisationSearch
{
    public class WhenOrganisationSearchByUkprnIsCalled : OrganisationSearchTestBase
    {
        [TestCaseSource(nameof(ThenOrganisationFoundCorrectlyCases))]
        public async Task ThenUkprnFoundCorrectlyForApprovedOrganisations(int ukprn, List<ThenOrganisationFoundCorrectlyResult> expectedResults)
        {
            // Act
            var sut = new OrganisationSearchOrchestrator(_logger.Object, _roatpApiClient.Object, _referenceDataApiClient.Object, _mediator.Object, _regexHelper.Object);
            var results = await sut.OrganisationSearchByUkprn(ukprn);

            // Assert
            results.Select(p => new { p.Name, p.Ukprn, p.RoEPAOApproved }).Should().BeEquivalentTo(expectedResults);
        }

        public static object[] ThenOrganisationFoundCorrectlyCases =
        {
            new object[]
            {
                12345678,
                new List<ThenOrganisationFoundCorrectlyResult>
                {
                    new ThenOrganisationFoundCorrectlyResult { Name = "Blue Barns Limited", Ukprn = 12345678, RoEPAOApproved = true },
                }
            },
            new object[]
            {
                41235678,
                new List<ThenOrganisationFoundCorrectlyResult>
                {
                    new ThenOrganisationFoundCorrectlyResult { Name = "Green Grass Limited", Ukprn = 41235678, RoEPAOApproved = true}
                }
            },
            new object[]
            {
                61234578,
                new List<ThenOrganisationFoundCorrectlyResult>
                {
                    new ThenOrganisationFoundCorrectlyResult { Name = "Yellow Sun Limited", Ukprn = 61234578, RoEPAOApproved = false}
                }
            }
        };

        public class ThenOrganisationFoundCorrectlyResult
        {
            public string Name { get; set; }
            public int Ukprn { get; set; }
            public bool RoEPAOApproved { get; set; }
        }
    }
}
