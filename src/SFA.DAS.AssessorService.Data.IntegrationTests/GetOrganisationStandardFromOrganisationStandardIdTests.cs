using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    public class GetOrganisationStandardFromOrganisationStandardIdTests : TestBase
    {
        private OrganisationModel _organisation1;
        private readonly DatabaseService _databaseService = new DatabaseService();
        private RegisterQueryRepository _repository;
        private string _organisationId1;
        private OrganisationStandardModel _organisationStandard;
        private string _organisationIdCreated;
        private readonly int _standardCode = 5;
        private readonly int _orgStandardId = 50;
        private DeliveryAreaModel _deliveryArea1;
        private DeliveryAreaModel _deliveryArea2;
        private readonly int _deliveryAreaId1 = 1;
        private readonly int _deliveryAreaId2 = 2;
        private readonly string _standardUID1 = "ST0001_1.0";
        private readonly string _standardUID2 = "ST0001_1.1";

        [OneTimeSetUp]
        public void SetupOrganisationTests()
        {
            var databaseConnection = new SqlConnection(_databaseService.WebConfiguration.SqlConnectionString);
            var unitOfWork = new UnitOfWork(databaseConnection);

            _repository = new RegisterQueryRepository(unitOfWork);

            _organisationIdCreated = "EPA0987";
            _organisationId1 = "EPA0987";
            _organisation1 = new OrganisationModel
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now.AddYears(-1).Date,
                DeletedAt = null,
                EndPointAssessorName = "Name 1",
                EndPointAssessorOrganisationId = _organisationId1,
                EndPointAssessorUkprn = 876544,
                PrimaryContact = null,
                Status = "new",
                UpdatedAt = null,
                OrganisationTypeId = null,
                OrganisationData = null
            };

            _organisationStandard = new OrganisationStandardModel
            {
                Id = _orgStandardId,
                EndPointAssessorOrganisationId = _organisationIdCreated,
                StandardCode = _standardCode,
                Status = OrganisationStatus.New,
                EffectiveFrom = DateTime.Today.AddDays(-7),
                EffectiveTo = DateTime.Today.AddDays(10),
                DateStandardApprovedOnRegister = DateTime.Today.AddDays(-50),
                Comments = "comments go here"
            };

            _deliveryArea1 = new DeliveryAreaModel { Id = 10, Status = "Live", Area = "North West" };
            _deliveryArea2 = new DeliveryAreaModel { Id = 20, Status = "New", Area = "Some Other" };
            var deliveryAreas = new List<DeliveryAreaModel> { _deliveryArea1, _deliveryArea2 };

            DeliveryAreaHandler.InsertRecords(deliveryAreas);

            var deliveryArea1 = new OrganisationStandardDeliveryAreaModel
            {
                Id = _deliveryAreaId1,
                Comments = "comments 1",
                DeliveryAreaId = _deliveryArea1.Id,
                OrganisationStandardId = _orgStandardId,
                Status = OrganisationStatus.New,
            };

            var deliveryArea2 = new OrganisationStandardDeliveryAreaModel
            {
                Id = _deliveryAreaId2,
                Comments = "comments 1",
                DeliveryAreaId = _deliveryArea2.Id,
                OrganisationStandardId = _orgStandardId,
                Status = OrganisationStatus.New
            };

            var organistionStandardVersion1 = new OrganisationStandardVersionModel
            {
                OrganisationStandardId = _orgStandardId,
                StandardUId = _standardUID1,
                EffectiveFrom = DateTime.Now.AddMonths(-12),
                EffectiveTo = DateTime.Now.AddMonths(-3),
                Version = "1.0",
                DateVersionApproved = DateTime.Now.AddMonths(-12),
                Status = "Live"
            };

            var organistionStandardVersion2 = new OrganisationStandardVersionModel
            {
                OrganisationStandardId = _orgStandardId,
                StandardUId = _standardUID2,
                EffectiveFrom = DateTime.Now.AddMonths(-3),
                EffectiveTo = null,
                Version = "1.1",
                DateVersionApproved = DateTime.Now.AddMonths(-3),
                Status = "Live"
            };

            var standardModel1 = new StandardModel
            {
                StandardUId = _standardUID1,
                IFateReferenceNumber = "ST0001",
                Version = "1.0",
                Title = "Standard",
                Level = 4,
                Status = "Approved for Delivery",
                TypicalDuration = 12,
                TrailblazerContact = "Contact name",
                StandardPageUrl = "www.standard.com",
                OverviewOfRole = "Explanation of apprenticeship job role",
                VersionApprovedForDelivery = DateTime.Now.AddMonths(-3)
            };

            var standardModel2 = new StandardModel
            {
                StandardUId = _standardUID2,
                IFateReferenceNumber = "ST0001",
                Version = "1.1",
                Title = "Standard",
                Level = 4,
                Status = "Approved for Delivery",
                TypicalDuration = 12,
                TrailblazerContact = "Contact name",
                StandardPageUrl = "www.standard.com",
                OverviewOfRole = "Explanation of apprenticeship job role",
                VersionApprovedForDelivery = DateTime.Now.AddMonths(-3)
            };

            OrganisationHandler.InsertRecords(new List<OrganisationModel> { _organisation1 });
            OrganisationStandardHandler.InsertRecord(_organisationStandard);
            OrganisationStandardDeliveryAreaHandler.InsertRecords(new List<OrganisationStandardDeliveryAreaModel>{
                deliveryArea1,deliveryArea2
            });
            OrganisationStandardVersionHandler.InsertRecord(organistionStandardVersion1);
            OrganisationStandardVersionHandler.InsertRecord(organistionStandardVersion2);

            StandardsHandler.InsertRecords(new List<StandardModel> { standardModel1, standardModel2 });
        }

        [TestCase(50, 1, 2)]
        [TestCase(51, 0, 0)]
        public async Task RunGetOrganisationStandardFromOrganisationStandardId(int organisationStandardId, int expectedCount, int expectedVersionCount)
        {
            var organisationStandardReturned = await _repository.GetOrganisationStandardFromOrganisationStandardId(organisationStandardId);

            if (expectedCount > 0)
            {
                Assert.IsNotNull(organisationStandardReturned, "Expected OrganisationStandardRecord but found none");

                if (expectedVersionCount > 0)
                {
                    Assert.AreEqual(expectedVersionCount, organisationStandardReturned.Versions.Count(), $@"Expected {expectedVersionCount} organisations back but got {organisationStandardReturned.Versions.Count()}");
                }
            }
            else
            {
                Assert.IsNull(organisationStandardReturned, "Did not expect OrganisationStandardRecord but found one");
            }


        }

        [OneTimeTearDown]
        public void TearDownOrganisationStandardTests()
        {
            OrganisationStandardVersionHandler.DeleteRecord(_orgStandardId, _standardUID1);
            OrganisationStandardVersionHandler.DeleteRecord(_orgStandardId, _standardUID2);
            OrganisationStandardDeliveryAreaHandler.DeleteRecords(new List<int> { _deliveryAreaId1, _deliveryAreaId2 });
            OrganisationStandardHandler.DeleteRecord(_orgStandardId);
            OrganisationHandler.DeleteRecords(new List<Guid> { _organisation1.Id });
            StandardsHandler.DeleteAllRecords();
        }
    }
}
