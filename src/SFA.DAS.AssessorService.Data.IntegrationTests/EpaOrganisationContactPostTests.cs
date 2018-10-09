using System;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    public class EpaOrganisationContactPostTests: TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private RegisterRepository _repository;
        private RegisterValidationRepository _validationRepository;
        private string _organisationIdCreated;
        private int _ukprnCreated;
        private OrganisationModel _organisation;
        private int _organisationTypeId;
        private Guid _id;
        private OrganisationModel _organisation2;
        private string _org2IdCreated;
        private Guid _contactId;
        private EpaContact _contact;
        private string _username;


        [OneTimeSetUp]
        public void SetUpOrganisationTests()
        {
            _repository = new RegisterRepository(_databaseService.WebConfiguration);
            _validationRepository = new RegisterValidationRepository(_databaseService.WebConfiguration);
            _organisationIdCreated = "EPA0987";
            _ukprnCreated = 123321;
            _org2IdCreated = "EPA0001";
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
                OrganisationTypeId = _organisationTypeId,
                OrganisationData = null,
                Status = OrganisationStatus.New
            };

            _organisation2 = new OrganisationModel
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                EndPointAssessorName = "name 1",
                EndPointAssessorOrganisationId = _org2IdCreated,
                OrganisationTypeId = null,
                Status = OrganisationStatus.New
            };

            _username = "username-9999";
            OrganisationHandler.InsertRecord(_organisation);
            OrganisationHandler.InsertRecord(_organisation2);

            _contactId = Guid.NewGuid();

            _contact = new EpaContact
            {
                Id = _contactId,
                EndPointAssessorOrganisationId = _organisationIdCreated,
                Username = _username,
                DisplayName = "Joe Cool",
                Email = "tester@test.com",
                PhoneNumber = "555 55555",
                Status = OrganisationStatus.New
            };
        }

        [Test]
        public void CreateOrganisationContactThatDoesntExistAndCheckItIsThere()
        {

            var isContactPresentBeforeInsert =
                _validationRepository.ContactIdIsValidForOrganisationId(_contactId.ToString(),_contact.EndPointAssessorOrganisationId).Result;
            var returnedUserName = _repository.CreateEpaOrganisationContact(_contact).Result;
            var isContactPresenAfterInsert =
                _validationRepository.ContactIdIsValidForOrganisationId(_contactId.ToString(), _contact.EndPointAssessorOrganisationId).Result;
         
            Assert.IsFalse(isContactPresentBeforeInsert);
            Assert.IsTrue(isContactPresenAfterInsert);
            Assert.AreEqual(returnedUserName, _contact.Username);
        }

        [OneTimeTearDown]
        public void TearDownOrganisationTests()
        {
            OrganisationContactHandler.DeleteRecordByUserName(_username);
            OrganisationHandler.DeleteRecordByOrganisationId(_organisationIdCreated);
            OrganisationHandler.DeleteRecordByOrganisationId(_org2IdCreated);
            OrganisationTypeHandler.DeleteRecord(_organisationTypeId);
        }
    }
}
