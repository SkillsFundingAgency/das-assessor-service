using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Register.Command
{
    [TestFixture]
    public class RegisterCreateOrganisationContactHandlerTests
    {
        private Mock<IRegisterRepository> _registerRepository;
        private CreateEpaOrganisationContactHandler _createEpaOrganisationContactHandler;
        private Mock<ISpecialCharacterCleanserService> _cleanserService;
        private Mock<IEpaOrganisationValidator> _validator;
        private Mock<ILogger<CreateEpaOrganisationContactHandler>> _logger;
        private Mock<IEpaOrganisationIdGenerator> _idGenerator;
        private CreateEpaOrganisationContactRequest _requestNoIssues;
        private string _organisationId;
        private string _displayName;
        private string _email;
        private string _phoneNumber;
        private EpaContact _expectedOrganisationContactNoIssues;
        //private int _requestNoIssuesId;
        private string _requestNoIssuesUserName;

        [SetUp]
        public void Setup()
        {
            _registerRepository = new Mock<IRegisterRepository>();
            _cleanserService = new Mock<ISpecialCharacterCleanserService>();
            _validator = new Mock<IEpaOrganisationValidator>();
            _logger = new Mock<ILogger<CreateEpaOrganisationContactHandler>>();
            _idGenerator = new Mock<IEpaOrganisationIdGenerator>();
            _organisationId = "EPA999";
            _displayName = "Testy McTestFace";
            _email = "testy@testface.com";
            _phoneNumber = "555 5555";
            //_requestNoIssuesId = 1;
            _requestNoIssuesUserName = "username-9999";
            _requestNoIssues = BuildRequest(_organisationId, _displayName,_email,_phoneNumber);
            _expectedOrganisationContactNoIssues = BuildOrganisationContact(_requestNoIssues, _requestNoIssuesUserName);

            _registerRepository.Setup(r => r.CreateEpaOrganisationContact(It.IsAny<EpaContact>()))
                .Returns(Task.FromResult(_expectedOrganisationContactNoIssues.Username));

            _validator.Setup(v => v.CheckIfOrganisationNotFound(_requestNoIssues.EndPointAssessorOrganisationId)).Returns(string.Empty);
            _validator.Setup(v => v.CheckIfDisplayNameIsMissing(_requestNoIssues.DisplayName)).Returns(string.Empty);
            _validator.Setup(v => v.CheckIfEmailIsMissing(_requestNoIssues.Email)).Returns(string.Empty);
            _cleanserService.Setup(c => c.CleanseStringForSpecialCharacters(It.IsAny<string>()))
                .Returns((string s) => s);
            _idGenerator.Setup(i => i.GetNextContactUsername()).Returns(_requestNoIssuesUserName);

            _createEpaOrganisationContactHandler = new CreateEpaOrganisationContactHandler(_registerRepository.Object, _validator.Object, _cleanserService.Object, _logger.Object, _idGenerator.Object);
        }

        //[Test]
        //public void CreateOrganisationStandardDetailsRepoIsCalledWhenHandlerInvoked()
        //{
        //    var res = _createEpaOrganisationContactHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
        //    _registerRepository.Verify(r => r.CreateEpaOrganisationStandard(It.IsAny<EpaOrganisationStandard>()));
        //}

        //[Test]
        //public void CheckAllValidatorsAreCalledWhenHandlerInvoked()
        //{
        //    var res = _createEpaOrganisationContactHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
        //    _validator.Verify(v => v.CheckOrganisationIdIsPresentAndValid(_requestNoIssues.OrganisationId));
        //    _validator.Verify(v => v.CheckIfOrganisationNotFound(_requestNoIssues.OrganisationId));
        //    _validator.Verify(v => v.CheckIfStandardNotFound(_requestNoIssues.StandardCode));
        //    _validator.Verify(v => v.CheckIfOrganisationStandardAlreadyExists(_requestNoIssues.OrganisationId, _requestNoIssues.StandardCode));
        //}

        //[Test]
        //public void GetOrganisationStandardWhenOrganisationStandardCreated()
        //{
        //    var returnedId = _createEpaOrganisationContactHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
        //    returnedId.Should().Be(_expectedOrganisationStandardNoIssues.Id.ToString());
        //}

        //[Test]
        //public void GetBadRequestExceptionWhenOrganisationIdIssueValidationOccurs()
        //{
        //    const string errorMessage = "no organisation Id";
        //    var requestNoOrgId = BuildRequest("", 1);
        //    _validator.Setup(v => v.CheckOrganisationIdIsPresentAndValid(requestNoOrgId.OrganisationId)).Returns(errorMessage);
        //    var ex = Assert.ThrowsAsync<BadRequestException>(() => _createEpaOrganisationContactHandler.Handle(requestNoOrgId, new CancellationToken()));
        //    Assert.AreEqual(errorMessage, ex.Message);
        //    _registerRepository.Verify(r => r.CreateEpaOrganisationStandard(It.IsAny<EpaOrganisationStandard>()), Times.Never);
        //    _validator.Verify(v => v.CheckOrganisationIdIsPresentAndValid(requestNoOrgId.OrganisationId));
        //}

        //[Test]
        //public void GetBadRequestExceptionWhenStandardCodeIssueValidationOccurs()
        //{
        //    const string errorMessage = "no standard code";
        //    var requestNoOrgId = BuildRequest("org id", -1);
        //    _validator.Setup(v => v.CheckOrganisationIdIsPresentAndValid(requestNoOrgId.OrganisationId)).Returns(errorMessage);
        //    var ex = Assert.ThrowsAsync<BadRequestException>(() => _createEpaOrganisationContactHandler.Handle(requestNoOrgId, new CancellationToken()));
        //    Assert.AreEqual(errorMessage, ex.Message);
        //    _registerRepository.Verify(r => r.CreateEpaOrganisationStandard(It.IsAny<EpaOrganisationStandard>()), Times.Never);
        //    _validator.Verify(v => v.CheckOrganisationIdIsPresentAndValid(requestNoOrgId.OrganisationId));
        //}

        //[Test]
        //public void GetBadRequestExceptionWhenNoOrganisationIdValidationOccurs()
        //{
        //    const string errorMessage = "No id issue";
        //    var requestNoOrgId = BuildRequest("orgid1", 123321);
        //    _validator.Setup(v => v.CheckIfOrganisationNotFound(requestNoOrgId.OrganisationId)).Returns(errorMessage);
        //    var ex = Assert.ThrowsAsync<NotFound>(() => _createEpaOrganisationContactHandler.Handle(requestNoOrgId, new CancellationToken()));
        //    Assert.AreEqual(errorMessage, ex.Message);
        //    _registerRepository.Verify(r => r.CreateEpaOrganisationStandard(It.IsAny<EpaOrganisationStandard>()), Times.Never);
        //    _validator.Verify(v => v.CheckIfOrganisationNotFound(requestNoOrgId.OrganisationId));
        //}

        //[Test]
        //public void GetBadRequestExceptionWhenNoStandardCodeValidationOccurs()
        //{
        //    const string errorMessage = "No id issue";
        //    var requestNoOrgId = BuildRequest("orgid1", 1);
        //    _validator.Setup(v => v.CheckIfStandardNotFound(requestNoOrgId.StandardCode)).Returns(errorMessage);
        //    var ex = Assert.ThrowsAsync<NotFound>(() => _createEpaOrganisationContactHandler.Handle(requestNoOrgId, new CancellationToken()));
        //    Assert.AreEqual(errorMessage, ex.Message);
        //    _registerRepository.Verify(r => r.CreateEpaOrganisationStandard(It.IsAny<EpaOrganisationStandard>()), Times.Never);
        //    _validator.Verify(v => v.CheckIfStandardNotFound(requestNoOrgId.StandardCode));
        //}

        //[Test]
        //public void GetBadRequestExceptionWhenOrganisationIdAlreadyExistsValidationOccurs()
        //{
        //    const string errorMessage = "id already exists";
        //    var requestOrgStandardlreadyExists = BuildRequest("EPA888", 123321);
        //    _validator.Setup(v => v.CheckIfOrganisationStandardAlreadyExists(requestOrgStandardlreadyExists.OrganisationId, requestOrgStandardlreadyExists.StandardCode)).Returns(errorMessage);
        //    var ex = Assert.ThrowsAsync<AlreadyExistsException>(() => _createEpaOrganisationContactHandler.Handle(requestOrgStandardlreadyExists, new CancellationToken()));
        //    Assert.AreEqual(errorMessage, ex.Message);
        //    _registerRepository.Verify(r => r.CreateEpaOrganisationStandard(It.IsAny<EpaOrganisationStandard>()), Times.Never);
        //    _validator.Verify(v => v.CheckIfOrganisationStandardAlreadyExists(requestOrgStandardlreadyExists.OrganisationId, requestOrgStandardlreadyExists.StandardCode));
        //}

        //[Test]
        //public void GetBadRequestExceptionWhenContactIdDoesntExistsValidationOccurs()
        //{
        //    const string errorMessage = "bad contact id";
        //    var requestOrgBadContactId = BuildRequest("EPA888", 123321);
        //    requestOrgBadContactId.ContactId = "badContactId";
        //    _validator.Setup(v => v.CheckIfContactIdIsEmptyOrValid(requestOrgBadContactId.ContactId, requestOrgBadContactId.OrganisationId)).Returns(errorMessage);
        //    var ex = Assert.ThrowsAsync<BadRequestException>(() => _createEpaOrganisationContactHandler.Handle(requestOrgBadContactId, new CancellationToken()));
        //    Assert.AreEqual(errorMessage, ex.Message);
        //    _validator.Verify(v => v.CheckIfContactIdIsEmptyOrValid(requestOrgBadContactId.ContactId, requestOrgBadContactId.OrganisationId));
        //}

        private CreateEpaOrganisationContactRequest BuildRequest(string organisationId, string displayName, string email, string phoneNumber)
        {
            return new CreateEpaOrganisationContactRequest
            {
                EndPointAssessorOrganisationId = organisationId,
                DisplayName = displayName,
                Email = email,
                PhoneNumber = phoneNumber
            };
        }

        private EpaContact BuildOrganisationContact(CreateEpaOrganisationContactRequest request, string username)
        {
            return new EpaContact
            {
                Id = Guid.NewGuid(),
                EndPointAssessorOrganisationId = request.EndPointAssessorOrganisationId,
                DisplayName = request.DisplayName,
                Username = username,
                Email = request.Email,
                Status = "Live",
                PhoneNumber = request.PhoneNumber

            };
        }
    }
}
