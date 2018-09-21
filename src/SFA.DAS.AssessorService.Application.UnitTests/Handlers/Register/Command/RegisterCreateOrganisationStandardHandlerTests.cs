using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Register.Command
{
    [TestFixture]
    public class RegisterCreateOrganisationStandardHandlerTests
    {
        private Mock<IRegisterRepository> _registerRepository;
        private CreateEpaOrganisationStandardHandler _createEpaOrganisationStandardHandler;
        private Mock<ISpecialCharacterCleanserService> _cleanserService;
        private Mock<IEpaOrganisationValidator> _validator;
        private Mock<ILogger<CreateEpaOrganisationStandardHandler>> _logger;
        private CreateEpaOrganisationStandardRequest _requestNoIssues;
        private string _organisationId;
        private EpaOrganisationStandard _expectedOrganisationStandardNoIssues;
        private int _requestNoIssuesId;

        [SetUp]
        public void Setup()
        {
            _registerRepository = new Mock<IRegisterRepository>();
            _cleanserService = new Mock<ISpecialCharacterCleanserService>();
            _validator = new Mock<IEpaOrganisationValidator>();
            _logger = new Mock<ILogger<CreateEpaOrganisationStandardHandler>>();
            _organisationId = "EPA999";
            _requestNoIssuesId = 1;

            _requestNoIssues = BuildRequest(_organisationId, 123321);
            _expectedOrganisationStandardNoIssues = BuildOrganisationStandard(_requestNoIssues,_requestNoIssuesId);

            _registerRepository.Setup(r => r.CreateEpaOrganisationStandard(It.IsAny<EpaOrganisationStandard>()))
                .Returns(Task.FromResult(_expectedOrganisationStandardNoIssues.Id.ToString()));

            _validator.Setup(v => v.CheckOrganisationIdIsPresentAndValid(_requestNoIssues.OrganisationId)).Returns(string.Empty);
            _validator.Setup(v => v.CheckIfOrganisationNotFound(_requestNoIssues.OrganisationId)).Returns(string.Empty);
            _validator.Setup(v => v.CheckIfStandardNotFound(_requestNoIssues.StandardCode)).Returns(string.Empty);
            _validator.Setup(v => v.CheckIfOrganisationStandardAlreadyExists(_requestNoIssues.OrganisationId, _requestNoIssues.StandardCode)).Returns(string.Empty);
            _cleanserService.Setup(c => c.CleanseStringForSpecialCharacters(It.IsAny<string>()))
                .Returns((string s) => s);
            
            _createEpaOrganisationStandardHandler = new CreateEpaOrganisationStandardHandler(_registerRepository.Object, _validator.Object, _logger.Object, _cleanserService.Object);
        }

        [Test]
        public void CreateOrganisationStandardDetailsRepoIsCalledWhenHandlerInvoked()
        {
            var res = _createEpaOrganisationStandardHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            _registerRepository.Verify(r => r.CreateEpaOrganisationStandard(It.IsAny<EpaOrganisationStandard>()));
        }

        [Test]
        public void CheckAllValidatorsAreCalledWhenHandlerInvoked()
        {
            var res = _createEpaOrganisationStandardHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            _validator.Verify(v => v.CheckOrganisationIdIsPresentAndValid(_requestNoIssues.OrganisationId));
            _validator.Verify(v => v.CheckIfOrganisationNotFound(_requestNoIssues.OrganisationId));
            _validator.Verify(v => v.CheckIfStandardNotFound(_requestNoIssues.StandardCode));
            _validator.Verify(v => v.CheckIfOrganisationStandardAlreadyExists(_requestNoIssues.OrganisationId, _requestNoIssues.StandardCode));
        }

        [Test]
        public void GetOrganisationStandardWhenOrganisationStandardCreated()
        {
            var returnedId = _createEpaOrganisationStandardHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            returnedId.Should().Be(_expectedOrganisationStandardNoIssues.Id.ToString());
        }

        [Test]
        public void GetBadRequestExceptionWhenOrganisationIdIssueValidationOccurs()
        {
            const string errorMessage = "no organisation Id";
            var requestNoOrgId = BuildRequest("", 1);
            _validator.Setup(v => v.CheckOrganisationIdIsPresentAndValid(requestNoOrgId.OrganisationId)).Returns(errorMessage);
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _createEpaOrganisationStandardHandler.Handle(requestNoOrgId, new CancellationToken()));
            Assert.AreEqual(errorMessage, ex.Message);
            _registerRepository.Verify(r => r.CreateEpaOrganisationStandard(It.IsAny<EpaOrganisationStandard>()), Times.Never);
            _validator.Verify(v => v.CheckOrganisationIdIsPresentAndValid(requestNoOrgId.OrganisationId));
        }

        [Test]
        public void GetBadRequestExceptionWhenStandardCodeIssueValidationOccurs()
        {
            const string errorMessage = "no standard code";
            var requestNoOrgId = BuildRequest("org id", -1);
            _validator.Setup(v => v.CheckOrganisationIdIsPresentAndValid(requestNoOrgId.OrganisationId)).Returns(errorMessage);
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _createEpaOrganisationStandardHandler.Handle(requestNoOrgId, new CancellationToken()));
            Assert.AreEqual(errorMessage, ex.Message);
            _registerRepository.Verify(r => r.CreateEpaOrganisationStandard(It.IsAny<EpaOrganisationStandard>()), Times.Never);
            _validator.Verify(v => v.CheckOrganisationIdIsPresentAndValid(requestNoOrgId.OrganisationId));
        }

        [Test]
        public void GetBadRequestExceptionWhenNoOrganisationIdValidationOccurs()
        {
            const string errorMessage = "No id issue";
            var requestNoOrgId = BuildRequest("orgid1", 123321);
            _validator.Setup(v => v.CheckIfOrganisationNotFound(requestNoOrgId.OrganisationId)).Returns(errorMessage);
            var ex = Assert.ThrowsAsync<NotFound>(() => _createEpaOrganisationStandardHandler.Handle(requestNoOrgId, new CancellationToken()));
            Assert.AreEqual(errorMessage, ex.Message);
            _registerRepository.Verify(r => r.CreateEpaOrganisationStandard(It.IsAny<EpaOrganisationStandard>()), Times.Never);
            _validator.Verify(v => v.CheckIfOrganisationNotFound(requestNoOrgId.OrganisationId));
        }

        [Test]
        public void GetBadRequestExceptionWhenNoStandardCodeValidationOccurs()
        {
            const string errorMessage = "No id issue";
            var requestNoOrgId = BuildRequest("orgid1", 1);
            _validator.Setup(v => v.CheckIfStandardNotFound(requestNoOrgId.StandardCode)).Returns(errorMessage);
            var ex = Assert.ThrowsAsync<NotFound>(() => _createEpaOrganisationStandardHandler.Handle(requestNoOrgId, new CancellationToken()));
            Assert.AreEqual(errorMessage, ex.Message);
            _registerRepository.Verify(r => r.CreateEpaOrganisationStandard(It.IsAny<EpaOrganisationStandard>()), Times.Never);
            _validator.Verify(v => v.CheckIfStandardNotFound(requestNoOrgId.StandardCode));
        }

        [Test]
        public void GetBadRequestExceptionWhenOrganisationIdAlreadyExistsValidationOccurs()
        {
            const string errorMessage = "id already exists";
            var requestOrgStandardlreadyExists = BuildRequest("EPA888", 123321);
            _validator.Setup(v => v.CheckIfOrganisationStandardAlreadyExists(requestOrgStandardlreadyExists.OrganisationId, requestOrgStandardlreadyExists.StandardCode)).Returns(errorMessage);
            var ex = Assert.ThrowsAsync<AlreadyExistsException>(() => _createEpaOrganisationStandardHandler.Handle(requestOrgStandardlreadyExists, new CancellationToken()));
            Assert.AreEqual(errorMessage, ex.Message);
            _registerRepository.Verify(r => r.CreateEpaOrganisationStandard(It.IsAny<EpaOrganisationStandard>()), Times.Never);
            _validator.Verify(v => v.CheckIfOrganisationStandardAlreadyExists(requestOrgStandardlreadyExists.OrganisationId, requestOrgStandardlreadyExists.StandardCode));
        }

        [Test]
        public void GetBadRequestExceptionWhenContactIdDoesntExistsValidationOccurs()
        {
            const string errorMessage = "bad contact id";
            var requestOrgBadContactId = BuildRequest("EPA888", 123321);
            requestOrgBadContactId.ContactId = "badContactId";
            _validator.Setup(v => v.CheckIfContactIdIsEmptyOrValid(requestOrgBadContactId.ContactId, requestOrgBadContactId.OrganisationId)).Returns(errorMessage);
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _createEpaOrganisationStandardHandler.Handle(requestOrgBadContactId, new CancellationToken()));
            Assert.AreEqual(errorMessage, ex.Message);
            _validator.Verify(v => v.CheckIfContactIdIsEmptyOrValid(requestOrgBadContactId.ContactId, requestOrgBadContactId.OrganisationId));
        }

        private CreateEpaOrganisationStandardRequest BuildRequest(string organisationId, int standardCode)
        {
            return new CreateEpaOrganisationStandardRequest
            {          
                OrganisationId = organisationId,
                StandardCode = standardCode,
                EffectiveFrom = null
            };
        }

        private EpaOrganisationStandard BuildOrganisationStandard(CreateEpaOrganisationStandardRequest request, int id)
        {
            return new EpaOrganisationStandard
            {
                Id = id,
                OrganisationId = request.OrganisationId,
                StandardCode = request.StandardCode,
                EffectiveFrom = request.EffectiveFrom,
                EffectiveTo = request.EffectiveTo,
                Comments = request.Comments
            };
        }
    }
}
