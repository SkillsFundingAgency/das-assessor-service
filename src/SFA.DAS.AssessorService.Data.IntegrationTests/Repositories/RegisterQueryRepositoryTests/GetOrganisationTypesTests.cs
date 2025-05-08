using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Data.SqlClient;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories.RegisterQueryRepositoryTests
{
    public class GetOrganisationTypesTests : TestBase
    {
        [Test]
        public async Task GetOrganisationTypes_IncludesAllNonDeletedOrganisationTypes_WhenCalled()
        {
            using (var fixture = new GetOrganisationTypesTestsFixture())
            {
                var result = await fixture.GetOrganisationTypes();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(10)
                        .VerifyResultsContain("Awarding Organisations")
                        .VerifyResultsContain("Assessment Organisations")
                        .VerifyResultsContain("Trade Body")
                        .VerifyResultsContain("Professional Body")
                        .VerifyResultsContain("HEI")
                        .VerifyResultsContain("NSA or SSC")
                        .VerifyResultsContain("Training Provider")
                        .VerifyResultsContain("Public Sector")
                        .VerifyResultsContain("College")
                        .VerifyResultsContain("Academy or Free School");
                }
            }
        }

        private class GetOrganisationTypesTestsFixture : FixtureBase<GetOrganisationTypesTestsFixture>, IDisposable
        {
            private readonly DatabaseService _databaseService = new DatabaseService();
            private readonly SqlConnection _sqlConnection;

            private RegisterQueryRepository _repository;
            private IEnumerable<OrganisationType> _results;

            public GetOrganisationTypesTestsFixture()
            {
                _sqlConnection = new SqlConnection(_databaseService.SqlConnectionStringTest);
                _repository = new RegisterQueryRepository(new UnitOfWork(_sqlConnection));
            }

            public async Task<GetOrganisationTypesTestsFixture> GetOrganisationTypes()
            {
                _results = await _repository.GetOrganisationTypes();
                return this;
            }

            public GetOrganisationTypesTestsFixture VerifyNumberOfResults(int numberOfResults)
            {
                _results.Count().Should().Be(numberOfResults);
                return this;
            }

            public GetOrganisationTypesTestsFixture VerifyResultsContain(string type)
            {
                _results.Should().Contain(o => o.Type == type);
                return this;
            }
        }
    }
}
