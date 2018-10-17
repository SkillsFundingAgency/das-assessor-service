using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    public class EpaOrganisationStandardPostTests: TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private RegisterRepository _repository;
        private RegisterQueryRepository _queryRepository;
        private RegisterValidationRepository _validationRepository;
        private string _organisationIdCreated;
        private int _ukprnCreated;
        private OrganisationModel _organisation;
        private int _organisationTypeId;
        private Guid _id;
        private EpaOrganisationStandard _organisationStandard;
        private readonly int _standardCode = 5;

        [OneTimeSetUp]
        public void SetupOrganisationStandardTests()
        {
            _repository = new RegisterRepository(_databaseService.WebConfiguration);
            _queryRepository = new RegisterQueryRepository(_databaseService.WebConfiguration);
            _validationRepository = new RegisterValidationRepository(_databaseService.WebConfiguration);
            _organisationIdCreated = "EPA0987";
            _ukprnCreated = 123321;
            _organisationTypeId = 5;
            OrganisationTypeHandler.InsertRecord(new OrganisationTypeModel { Id = _organisationTypeId, Status = "new", Type = "organisation type 1" });
            _id = Guid.NewGuid();

            _organisation = new OrganisationModel
            {
                Id = _id,
                CreatedAt = DateTime.Now,
                EndPointAssessorName = "name 2",
                EndPointAssessorOrganisationId = _organisationIdCreated,
                EndPointAssessorUkprn = _ukprnCreated,
                PrimaryContact = null,
                Status = OrganisationStatus.New,
                OrganisationTypeId = _organisationTypeId,
                OrganisationData = null
            };

            OrganisationHandler.InsertRecord(_organisation);

            _organisationStandard = new EpaOrganisationStandard
            {
                OrganisationId = _organisationIdCreated,
                StandardCode = _standardCode,
                Status = OrganisationStatus.New,
                EffectiveFrom = DateTime.Today.AddDays(-7),
                EffectiveTo = DateTime.Today.AddDays(10),
                DateStandardApprovedOnRegister = DateTime.Today.AddDays(-50),
                Comments = "comments go here"
            };
        }

        [Test]
        public void CreateOrganisationThatDoesntExistAndCheckItIsThere()
        {
            var isOrgStandardPresentBeforeInsert = _validationRepository.EpaOrganisationStandardExists(_organisationIdCreated,_standardCode).Result;
            var returnedOrganisationStandardId = _repository.CreateEpaOrganisationStandard(_organisationStandard).Result;
            var isOrgStandardPresentAfterInsert = _validationRepository.EpaOrganisationStandardExists(_organisationIdCreated, _standardCode).Result;
            var returnedOrganisationStandardByIds = OrganisationStandardHandler.GetOrganisationStandardByOrgIdStandardCode(_organisationIdCreated, _standardCode);

            Assert.IsFalse(isOrgStandardPresentBeforeInsert);
            Assert.IsTrue(isOrgStandardPresentAfterInsert);
            Assert.AreEqual(returnedOrganisationStandardId, returnedOrganisationStandardByIds.Id.ToString());
        }

        [OneTimeTearDown]
        public void TearDownOrganisationTests()
        {
            OrganisationStandardHandler.DeleteRecordByOrganisationIdStandardCode(_organisationIdCreated, _standardCode);
            OrganisationHandler.DeleteRecordByOrganisationId(_organisationIdCreated);
            OrganisationTypeHandler.DeleteRecord(_organisationTypeId);
        }
    }
}
