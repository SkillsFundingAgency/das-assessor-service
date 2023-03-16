using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Orchestrators;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Orchestrators.OrganisationSearch
{
    public class WhenOrganisationSearchByEpao : OrganisationSearchTestBase
    {
        [TestCaseSource(nameof(ThenOrganisationFoundCorrectlyCases))]
        public async Task ThenOrganisationFoundCorrectly(string searchTerm, List<ThenOrganisationFoundCorrectlyResult> expectedResults)
        {
            // Act
            var sut = new OrganisationSearchOrchestrator(_logger.Object, _roatpApiClient.Object, _referenceDataApiClient.Object, _mediator.Object, _regexHelper.Object);
            var results = await sut.OrganisationSearchByEpao(searchTerm);

            // Assert
            results.Select(p => new { p.Name, p.Id, p.RoEPAOApproved }).Should().BeEquivalentTo(expectedResults);
        }

        public static object[] ThenOrganisationFoundCorrectlyCases =
        {
            new object[] 
            { 
                "EPA0001", 
                new List<ThenOrganisationFoundCorrectlyResult>
                {
                    new ThenOrganisationFoundCorrectlyResult { Name = "Blue Barns Limited", Id = "EPA0001", RoEPAOApproved = true },
                }
            },
            new object[] 
            {   
                "EPA0006", 
                new List<ThenOrganisationFoundCorrectlyResult> { }
            },
            new object[] 
            {   
                "EPA0004", 
                new List<ThenOrganisationFoundCorrectlyResult>
                {
                    new ThenOrganisationFoundCorrectlyResult { Name = "Green Grass Limited", Id = "EPA0004", RoEPAOApproved = true}
                }
            }
        };

        public class ThenOrganisationFoundCorrectlyResult
        {
            public string Name { get; set; }
            public string Id { get; set; }
            public bool RoEPAOApproved { get; set; }
        }
    }
}
