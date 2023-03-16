using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Orchestrators;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Orchestrators.OrganisationSearch
{
    public class WhenOrganisationSearchByNameOrCharityNumberOrCompanyNumberIsCalled : OrganisationSearchTestBase
    {
        [TestCaseSource(nameof(ThenOrganisationFoundCorrectlyCases))]
        public async Task ThenOrganisationFoundCorrectly(string searchTerm, List<ThenOrganisationFoundCorrectlyResult> expectedResults)
        {
            // Act
            var sut = new OrganisationSearchOrchestrator(_logger.Object, _roatpApiClient.Object, _referenceDataApiClient.Object, _mediator.Object, _regexHelper.Object);
            var results = await sut.OrganisationSearchByNameOrCharityNumberOrCompanyNumber(searchTerm);

            // Assert
            results.Select(p => new { p.Name, p.CompanyNumber, p.RoEPAOApproved }).Should().BeEquivalentTo(expectedResults);
        }

        public static object[] ThenOrganisationFoundCorrectlyCases =
        {
            new object[] 
            { 
                "Blue", 
                new List<ThenOrganisationFoundCorrectlyResult>
                {
                    new ThenOrganisationFoundCorrectlyResult { Name = "Blue Barns Limited", CompanyNumber = "00030001", RoEPAOApproved = true },
                    new ThenOrganisationFoundCorrectlyResult { Name = "Sky Blue Ltd", CompanyNumber = "00030002", RoEPAOApproved = true }
                }
            },
            new object[] 
            {   
                "Yellow", 
                new List<ThenOrganisationFoundCorrectlyResult>
                {
                    new ThenOrganisationFoundCorrectlyResult { Name = "Yellow Sun Limited", CompanyNumber = "00030006", RoEPAOApproved = false}
                }
            },
            new object[] 
            {   
                "Green Grass Limited", 
                new List<ThenOrganisationFoundCorrectlyResult>
                {
                    new ThenOrganisationFoundCorrectlyResult { Name = "Green Grass Limited", CompanyNumber = "00030004", RoEPAOApproved = true}
                }
            },
            new object[] 
            {   
                "Green Trees Limited", // search for the new company name
                new List<ThenOrganisationFoundCorrectlyResult>
                {
                    new ThenOrganisationFoundCorrectlyResult { Name = "Green Grass Limited", CompanyNumber = "00030004", RoEPAOApproved = true} // find the previous company name
                }
            },
            new object[]
            {
                "Green", // search for partial match to new company name
                new List<ThenOrganisationFoundCorrectlyResult>
                {
                    new ThenOrganisationFoundCorrectlyResult { Name = "Green Grass Limited", CompanyNumber = "00030004", RoEPAOApproved = true}, // find the previous company name
                    new ThenOrganisationFoundCorrectlyResult { Name = "Green Bush Ltd", CompanyNumber = "00030007", RoEPAOApproved = false}
                }
            }
        };

        public class ThenOrganisationFoundCorrectlyResult
        {
            public string Name { get; set; }
            public string CompanyNumber { get; set; }
            public bool RoEPAOApproved { get; set; }
        }
    }
}
