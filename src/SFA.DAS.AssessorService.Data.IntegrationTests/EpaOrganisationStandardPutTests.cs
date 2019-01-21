// Mark Cain Commented out cos causing a 'Enlisting in Ambient transactions is not supported'
// This appears to be a known bug in .NetCore 2, and may be fixed soon - so I want to uncomment it in a few months to see what happens

//using System;
//using System.Collections.Generic;
//using System.Text;
//using NUnit.Framework;
//using SFA.DAS.AssessorService.Api.Types.Models.AO;
//using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
//using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
//using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
//using SFA.DAS.AssessorService.Domain.Consts;
//
//namespace SFA.DAS.AssessorService.Data.IntegrationTests
//{
//    public class EpaOrganisationStandardPutTests : TestBase
//    {
//        private readonly DatabaseService _databaseService = new DatabaseService();
//        private RegisterRepository _repository;
//        private RegisterQueryRepository _queryRepository;
//        private RegisterValidationRepository _validationRepository;
//        private string _organisationIdCreated;
//        private int _ukprnCreated;
//        private OrganisationModel _organisation;
//        private int _organisationTypeId;
//        private Guid _id;
//        private EpaOrganisationStandard _organisationStandardUpdates;
//        private readonly int _standardCode = 5;
//        private OrganisationStandardModel _orgStandardModel;
//        private bool _isOrgStandardPresentBeforeUpdate;
//        private EpaOrganisationStandard _organisationStandardBeforeUpdate;
//        private string _returnedOrganisationStandardId;
//        private EpaOrganisationStandard _organisationStandardAfterUpdate;
//        private List<int> _deliveryAreas;
//        private DeliveryAreaModel _deliveryArea1;
//        private DeliveryAreaModel _deliveryArea2;
//
//        [OneTimeSetUp]
//        public void SetupOrganisationStandardTests()
//        {
//            _repository = new RegisterRepository(_databaseService.WebConfiguration);
//            _queryRepository = new RegisterQueryRepository(_databaseService.WebConfiguration);
//            _validationRepository = new RegisterValidationRepository(_databaseService.WebConfiguration);
//            _organisationIdCreated = "EPA0987";
//            _ukprnCreated = 123321;
//            _organisationTypeId = 5;
//            OrganisationTypeHandler.InsertRecord(new OrganisationTypeModel { Id = _organisationTypeId, Status = "new", Type = "organisation type 1" });
//            _id = Guid.NewGuid();
//
//            _organisation = new OrganisationModel
//            {
//                Id = _id,
//                CreatedAt = DateTime.Now,
//                EndPointAssessorName = "name 2",
//                EndPointAssessorOrganisationId = _organisationIdCreated,
//                EndPointAssessorUkprn = _ukprnCreated,
//                PrimaryContact = null,
//                Status = OrganisationStatus.New,
//                OrganisationTypeId = _organisationTypeId,
//                OrganisationData = null
//            };
//
//            OrganisationHandler.InsertRecord(_organisation);
//            _orgStandardModel = new OrganisationStandardModel
//            {
//                EndPointAssessorOrganisationId = _organisationIdCreated,
//                StandardCode = _standardCode,
//                Status = OrganisationStatus.New,
//                EffectiveFrom = DateTime.Today.AddDays(-7),
//                EffectiveTo = DateTime.Today.AddDays(10),
//                DateStandardApprovedOnRegister = DateTime.Today.AddDays(-50),
//                Comments = "comments go here"
//            };
//            OrganisationStandardHandler.InsertRecord(_orgStandardModel);
//
//            _deliveryArea1 = new DeliveryAreaModel { Id = 1, Status = "Live", Area = "North West" };
//            _deliveryArea2 = new DeliveryAreaModel { Id = 2, Status = "New", Area = "Some Other" };
//            var deliveryAreas = new List<DeliveryAreaModel> { _deliveryArea1, _deliveryArea2 };
//
//            DeliveryAreaHandler.InsertRecords(deliveryAreas);
//            _deliveryAreas = new List<int> { 1, 2 };
//
//            _organisationStandardUpdates = new EpaOrganisationStandard
//            {
//                OrganisationId = _organisationIdCreated,
//                StandardCode = _standardCode,
//                EffectiveFrom = DateTime.Today.AddDays(5),
//                EffectiveTo = DateTime.Today.AddDays(115),
//                DateStandardApprovedOnRegister = null,
//                Comments = "comments updated"
//            };
//
//
//            _isOrgStandardPresentBeforeUpdate = _validationRepository.EpaOrganisationStandardExists(_organisationIdCreated, _standardCode).Result;
//            _organisationStandardBeforeUpdate = OrganisationStandardHandler.GetOrganisationStandardByOrgIdStandardCode(_organisationIdCreated, _standardCode);
//            _returnedOrganisationStandardId = _repository.UpdateEpaOrganisationStandard(_organisationStandardUpdates, _deliveryAreas).Result;
//            _organisationStandardAfterUpdate = OrganisationStandardHandler.GetOrganisationStandardByOrgIdStandardCode(_organisationIdCreated, _standardCode);
//
//        }
//
//        [Test]
//        public void UpdateOrganisationStandardCheckIsPresentBeforeUpdating()
//        {
//            Assert.IsTrue(_isOrgStandardPresentBeforeUpdate);
//        }
//
//        [Test]
//        public void UpdateOrganisationStandardAndCheckTheBeforeValuesAreAsExpected()
//        { 
//            Assert.AreEqual(_orgStandardModel.EffectiveFrom, _organisationStandardBeforeUpdate.EffectiveFrom);
//            Assert.AreEqual(_orgStandardModel.EffectiveTo, _organisationStandardBeforeUpdate.EffectiveTo);
//            Assert.AreEqual(_orgStandardModel.EffectiveFrom, _organisationStandardBeforeUpdate.EffectiveFrom);
//            Assert.AreEqual(_orgStandardModel.DateStandardApprovedOnRegister, _organisationStandardBeforeUpdate.DateStandardApprovedOnRegister);
//            Assert.AreEqual(_orgStandardModel.Comments, _organisationStandardBeforeUpdate.Comments);
//        }
//
//        [Test]
//        public void UpdateOrganisationStandardAndCheckAfterValuesAreAsExpected()
//        {
//            Assert.AreEqual(_returnedOrganisationStandardId, _organisationStandardAfterUpdate.Id.ToString());
//            Assert.AreEqual(_organisationStandardAfterUpdate.EffectiveFrom, _organisationStandardUpdates.EffectiveFrom);
//            Assert.AreEqual(_organisationStandardAfterUpdate.EffectiveTo, _organisationStandardUpdates.EffectiveTo);
//            Assert.AreNotEqual(_organisationStandardAfterUpdate.DateStandardApprovedOnRegister, _organisationStandardUpdates.DateStandardApprovedOnRegister);
//            Assert.AreEqual(_organisationStandardAfterUpdate.Comments, _organisationStandardUpdates.Comments);
//        }
//
//        [Test]
//        public void UpdateOrganisationStandardCheckBeforeAndAfterValuesHaveChanged()
//        {
//            Assert.AreNotEqual(_organisationStandardBeforeUpdate.EffectiveFrom, _organisationStandardAfterUpdate.EffectiveFrom);
//            Assert.AreNotEqual(_organisationStandardBeforeUpdate.EffectiveTo, _organisationStandardAfterUpdate.EffectiveTo);
//            Assert.AreEqual(_organisationStandardBeforeUpdate.DateStandardApprovedOnRegister, _organisationStandardAfterUpdate.DateStandardApprovedOnRegister);
//            Assert.AreNotEqual(_organisationStandardBeforeUpdate.Comments, _organisationStandardAfterUpdate.Comments);
//        }
//
//
//    [OneTimeTearDown]
//        public void TearDownOrganisationTests()
//        {
//            OrganisationStandardHandler.DeleteRecordByOrganisationIdStandardCode(_organisationIdCreated, _standardCode);
//            OrganisationHandler.DeleteRecordByOrganisationId(_organisationIdCreated);
//            OrganisationTypeHandler.DeleteRecord(_organisationTypeId);
//            DeliveryAreaHandler.DeleteRecords(new List<int> { _deliveryArea1.Id, _deliveryArea2.Id });
//        }
//    }
//}
