using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Data.SqlClient;

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
            var databaseConnection = new SqlConnection(_databaseService.SqlConnectionStringTest);
            var unitOfWork = new UnitOfWork(databaseConnection);

            _repository = new RegisterRepository(unitOfWork, new Mock<ILogger<RegisterRepository>>().Object);
            _validationRepository = new RegisterValidationRepository(unitOfWork);

            _organisationIdCreated = "EPA0987";
            _ukprnCreated = 123321;
            _org2IdCreated = "EPA0001";
            _organisationTypeId = 5;
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
                Status = OrganisationStatus.New,
                SigninType = "",
                FirstName = "zzz",
                LastName = "Ftagn"
               
            };
        }

        [Test]
        public void CreateOrganisationContactThatDoesntExistAndCheckItIsThere()
        {

            var isContactPresentBeforeInsert =
                _validationRepository.ContactIdIsValidForOrganisationId(_contactId,_contact.EndPointAssessorOrganisationId).Result;
            var returnedContactId = _repository.CreateEpaOrganisationContact(_contact).Result;
            var isContactPresenAfterInsert =
                _validationRepository.ContactIdIsValidForOrganisationId(_contactId, _contact.EndPointAssessorOrganisationId).Result;
         
            isContactPresentBeforeInsert.Should().BeFalse();
            isContactPresenAfterInsert.Should().BeTrue();
            returnedContactId.Should().Be(_contactId.ToString());
        }

        [OneTimeTearDown]
        public void TearDownOrganisationTests()
        {
            OrganisationContactHandler.DeleteRecordByUserName(_username);
            OrganisationHandler.DeleteRecordByEndPointAssessorOrganisationId(_organisationIdCreated);
            OrganisationHandler.DeleteRecordByEndPointAssessorOrganisationId(_org2IdCreated);
            OrganisationTypeHandler.DeleteRecord(_organisationTypeId);
        }
    }
}
