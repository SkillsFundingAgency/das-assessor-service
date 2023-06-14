using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories
{
    public class StandardRepositoryTests : TestBase
    {
        [Test]
        public async Task GetEpaoRegisteredStandards_ReturnsOrganisationStandardsWithVersions_WhenRequireAtLeastOneVersionIsDefault()
        {
            using (var fixture = new StandardRepositoryTestsFixture()
                .WithStandard("BrickLayer", "ST0001", 101, "1.0")
                .WithStandard("Bricklayer", "ST0001", 101, "1.1")
                .WithOrganisation("Brick & Co", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.1", null)
                .WithStandard("Roofer", "ST0002", 102, "1.0")
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", null)) // no versions are opted in
            {
                (await fixture.GetEpaoRegisteredStandards("EPA0001", true)).Verify(1);
            }
        }

        [Test]
        public async Task GetEpaoRegisteredStandards_ReturnsOrganisationStandardsWithVersions_WhenRequireAtLeastOneVersionIsTrue()
        {
            using (var fixture = new StandardRepositoryTestsFixture()
                .WithStandard("BrickLayer", "ST0001", 101, "1.0")
                .WithStandard("Bricklayer", "ST0001", 101, "1.1")
                .WithOrganisation("Brick & Co", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.1", null)
                .WithStandard("Roofer", "ST0002", 102, "1.0")
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", null)) // no versions are opted in
            {
                (await fixture.GetEpaoRegisteredStandards("EPA0001", true)).Verify(1);
            }
        }

        [Test]
        public async Task GetEpaoRegisteredStandards_ReturnsOrganisationStandardsWithOrWithoutVersions_WhenRequireAtLeastOneVersionIsFalse()
        {
            using (var fixture = new StandardRepositoryTestsFixture()
                .WithStandard("BrickLayer", "ST0001", 101, "1.0")
                .WithStandard("Bricklayer", "ST0001", 101, "1.1")
                .WithOrganisation("Brick & Co", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.1", null)
                .WithStandard("Roofer", "ST0002", 102, "1.0")
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", null)) // no versions are opted in
            {
                (await fixture.GetEpaoRegisteredStandards("EPA0001", false)).Verify(2);
            }
        }

        [Test]
        public async Task GetEpaoRegisteredStandards_ReturnsOrganisationStandardsNotRemoved_WhenRequireAtLeastOneVersionIsDefault()
        {
            using (var fixture = new StandardRepositoryTestsFixture()
                .WithStandard("BrickLayer", "ST0001", 101, "1.0")
                .WithStandard("Bricklayer", "ST0001", 101, "1.1")
                .WithOrganisation("Brick & Co", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DateTime.Today) // standard has been removed
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.1", null)
                .WithStandard("Roofer", "ST0002", 102, "1.0")
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", null)) // no versions are opted in
            {
                (await fixture.GetEpaoRegisteredStandards("EPA0001")).Verify(0);
            }
        }

        [Test]
        public async Task GetEpaoRegisteredStandards_ReturnsOrganisationStandardsNotRemoved_WhenRequireAtLeastOneVersionIsTrue()
        {
            using (var fixture = new StandardRepositoryTestsFixture()
                .WithStandard("BrickLayer", "ST0001", 101, "1.0")
                .WithStandard("Bricklayer", "ST0001", 101, "1.1")
                .WithOrganisation("Brick & Co", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DateTime.Today) // standard has been removed
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.1", null)
                .WithStandard("Roofer", "ST0002", 102, "1.0")
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", null)) // no versions are opted in
            {
                (await fixture.GetEpaoRegisteredStandards("EPA0001", true)).Verify(0);
            }
        }

        [Test]
        public async Task GetEpaoRegisteredStandards_ReturnsOrganisationStandardsNotRemoved_WhenRequireAtLeastOneVersionIsFalse()
        {
            using (var fixture = new StandardRepositoryTestsFixture()
                .WithStandard("BrickLayer", "ST0001", 101, "1.0")
                .WithStandard("Bricklayer", "ST0001", 101, "1.1")
                .WithOrganisation("Brick & Co", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DateTime.Today) // standard has been removed
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.1", null)
                .WithStandard("Roofer", "ST0002", 102, "1.0")
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", null)) // no versions are opted in
            {
                (await fixture.GetEpaoRegisteredStandards("EPA0001", false)).Verify(1);
            }
        }

        [Test]
        public async Task GetEpaoRegisteredStandards_ReturnsOrganisationStandardsWithVersionsWithVersionsOptedIn_WhenRequireAtLeastOneVersionIsDefault()
        {
            using (var fixture = new StandardRepositoryTestsFixture()
                .WithStandard("BrickLayer", "ST0001", 101, "1.0")
                .WithStandard("Bricklayer", "ST0001", 101, "1.1")
                .WithOrganisation("Brick & Co", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", null) 
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.1", null)
                .WithStandard("Roofer", "ST0002", 102, "1.0")
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", null)
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DateTime.Today)) // version is opted out
            {
                (await fixture.GetEpaoRegisteredStandards("EPA0001")).Verify(1);
            }
        }

        [Test]
        public async Task GetEpaoRegisteredStandards_ReturnsOrganisationStandardsWithVersionsWithVersionsOptedIn_WhenRequireAtLeastOneVersionIsTrue()
        {
            using (var fixture = new StandardRepositoryTestsFixture()
                .WithStandard("BrickLayer", "ST0001", 101, "1.0")
                .WithStandard("Bricklayer", "ST0001", 101, "1.1")
                .WithOrganisation("Brick & Co", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", null) 
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.1", null)
                .WithStandard("Roofer", "ST0002", 102, "1.0")
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", null)
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DateTime.Today)) // version is opted out
            {
                (await fixture.GetEpaoRegisteredStandards("EPA0001", true)).Verify(1);
            }
        }

        [Test]
        public async Task GetEpaoRegisteredStandards_ReturnsOrganisationStandardsWithVersionsOptedInOrOut_WhenRequireAtLeastOneVersionIsFalse()
        {
            using (var fixture = new StandardRepositoryTestsFixture()
                .WithStandard("BrickLayer", "ST0001", 101, "1.0")
                .WithStandard("Bricklayer", "ST0001", 101, "1.1")
                .WithOrganisation("Brick & Co", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", null) 
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.1", null)
                .WithStandard("Roofer", "ST0002", 102, "1.0")
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", null)
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DateTime.Today)) // version is opted out
            {
                (await fixture.GetEpaoRegisteredStandards("EPA0001", false)).Verify(2);
            }
        }

        private class StandardRepositoryTestsFixture: IDisposable
        {
            private readonly DatabaseService _databaseService = new DatabaseService();
            private readonly SqlConnection _sqlConnection;

            private StandardRepository _repository;
            private EpoRegisteredStandardsResult _result;

            private List<StandardModel> _standards = new List<StandardModel>();
            private List<OrganisationModel> _organisations = new List<OrganisationModel>();
            private List<OrganisationStandardModel> _organisationStandards = new List<OrganisationStandardModel>();
            private List<OrganisationStandardVersionModel> _organisationStandardVersions = new List<OrganisationStandardVersionModel>();

            public StandardRepositoryTestsFixture()
            {
                _sqlConnection = new SqlConnection(_databaseService.WebConfiguration.SqlConnectionString);
                _repository = new StandardRepository(new UnitOfWork(_sqlConnection));

                // this is to workaround the other tests which are not clearing up after themselves properly
                DeleteAllRecords();
            }

            public async Task<StandardRepositoryTestsFixture> GetEpaoRegisteredStandards(string endpointAssessmentOrganisationId)
            {
                _result = await _repository.GetEpaoRegisteredStandards(endpointAssessmentOrganisationId, 10, 1);
                return this;
            }

            public async Task<StandardRepositoryTestsFixture> GetEpaoRegisteredStandards(string endpointAssessmentOrganisationId, bool requireAtLeastOneVersion)
            {
                _result = await _repository.GetEpaoRegisteredStandards(endpointAssessmentOrganisationId, requireAtLeastOneVersion, 10, 1);
                return this;
            }

            public void Verify(int numberOfResults)
            {
                Assert.That(_result.PageOfResults.Count, Is.EqualTo(numberOfResults));
            }

            public StandardRepositoryTestsFixture WithOrganisation(string endPointAssessorName, string endPointAssessorOrganisationId, int ukprn)
            {
                var organisation = new OrganisationModel
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    EndPointAssessorName = endPointAssessorName,
                    EndPointAssessorOrganisationId = endPointAssessorOrganisationId,
                    EndPointAssessorUkprn = ukprn,
                    PrimaryContact = null,
                    OrganisationTypeId = 1,
                    OrganisationData = null,
                    Status = OrganisationStatus.Live
                };

                _organisations.Add(organisation);
                OrganisationHandler.InsertRecord(organisation);

                return this;
            }

            public StandardRepositoryTestsFixture WithStandard(string title, string referenceNumber, int larsCode, string version) 
            {
                var standard = new StandardModel
                {
                    Title = title,
                    IFateReferenceNumber = referenceNumber,
                    LarsCode = larsCode,
                    StandardUId = $"{referenceNumber}_{version}",
                    Version = version,
                    Level = 4,
                    Status = "Approved for delivery",
                    TypicalDuration = 12,
                    TrailblazerContact = "TrailblazerContact",
                    StandardPageUrl = "www.standard.com",
                    OverviewOfRole = "OverviewOfRole",
                    VersionApprovedForDelivery = DateTime.Now.AddDays(-50)
                };

                _standards.Add(standard);
                StandardsHandler.InsertRecord(standard);

                return this;
            }

            public StandardRepositoryTestsFixture WithOrganisationStandard(int id, string endPointAssessorOrganisationId, int larsCode, string standardReference, DateTime? effectiveTo)
            {
                var organisationStandard = new OrganisationStandardModel
                {
                    Id = id,
                    EndPointAssessorOrganisationId = endPointAssessorOrganisationId,
                    StandardCode = larsCode,
                    Status = OrganisationStatus.Live,
                    EffectiveFrom = DateTime.Today.AddDays(-10),
                    EffectiveTo = effectiveTo,
                    DateStandardApprovedOnRegister = DateTime.Today.AddDays(-50),
                    Comments = string.Empty,
                    StandardReference = standardReference,
                };

                _organisationStandards.Add(organisationStandard);
                OrganisationStandardHandler.InsertRecord(organisationStandard);
                
                return this;
            }

            public StandardRepositoryTestsFixture WithOrganisationStandardVersion(string endPointAssessorOrganisationId, int larsCode, string standardReference, string version, DateTime? effectiveTo)
            {
                var organisationStandard = _organisationStandards.First(p => p.EndPointAssessorOrganisationId == endPointAssessorOrganisationId && p.StandardCode == larsCode);
                var standard = _standards.First(p => p.IFateReferenceNumber == standardReference && p.Version == version);

                var organisationStandardVersion = new OrganisationStandardVersionModel
                {
                    OrganisationStandardId = organisationStandard.Id,
                    StandardUId = standard.StandardUId,
                    EffectiveFrom = DateTime.Now.AddDays(-5),
                    EffectiveTo = effectiveTo,
                    Version = standard.Version,
                    DateVersionApproved = DateTime.Now.AddMonths(-12),
                    Status = OrganisationStatus.Live
                };

                _organisationStandardVersions.Add(organisationStandardVersion);
                OrganisationStandardVersionHandler.InsertRecord(organisationStandardVersion);

                return this;
            }

            public void Dispose()
            {
                DeleteAllRecords();
            }

            private void DeleteAllRecords()
            {
                OrganisationStandardVersionHandler.DeleteAllRecords();
                OrganisationStandardHandler.DeleteAllRecords();
                OrganisationHandler.DeleteAllRecords();
                StandardsHandler.DeleteAllRecords();
            }
        }
    }
}
