using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories.RegisterQueryRepositoryTests
{
    public class GetDeliveryAreasTests : TestBase
    {
        [Test]
        public async Task GetDeliveryAreas_IncludesAllDeliveryAreas_WhenCalled()
        {
            using (var fixture = new GetDeliveryAreasTestsFixture())
            {
                var result = await fixture.GetDeliveryAreas();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(9)
                        .VerifyResultsContain("East Midlands")
                        .VerifyResultsContain("East of England")
                        .VerifyResultsContain("London")
                        .VerifyResultsContain("North East")
                        .VerifyResultsContain("North West")
                        .VerifyResultsContain("South East")
                        .VerifyResultsContain("South West")
                        .VerifyResultsContain("West Midlands")
                        .VerifyResultsContain("Yorkshire and the Humber");
                }
            }
        }

        private class GetDeliveryAreasTestsFixture : FixtureBase<GetDeliveryAreasTestsFixture>, IDisposable
        {
            private readonly DatabaseService _databaseService = new DatabaseService();
            private readonly SqlConnection _sqlConnection;

            private RegisterQueryRepository _repository;
            private IEnumerable<DeliveryArea> _results;

            public GetDeliveryAreasTestsFixture()
            {
                _sqlConnection = new SqlConnection(_databaseService.WebConfiguration.SqlConnectionString);
                _repository = new RegisterQueryRepository(new UnitOfWork(_sqlConnection));
            }

            public async Task<GetDeliveryAreasTestsFixture> GetDeliveryAreas()
            {
                _results = await _repository.GetDeliveryAreas();
                return this;
            }

            public GetDeliveryAreasTestsFixture VerifyNumberOfResults(int numberOfResults)
            {
                _results.Count().Should().Be(numberOfResults);
                return this;
            }

            public GetDeliveryAreasTestsFixture VerifyResultsContain(string area)
            {
                _results.Should().Contain(o => o.Area == area);
                return this;
            }
        }
    }
}
