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
            _validator = new Mock<IEpaOrganisationValidator>();
            _logger = new Mock<ILogger<CreateEpaOrganisationStandardHandler>>();
            _organisationId = "EPA999";
            _requestNoIssuesId = 1;

            _requestNoIssues = BuildRequest(_organisationId, 123321);
            _expectedOrganisationStandardNoIssues = BuildOrganisationStandard(_requestNoIssues,_requestNoIssuesId);

            _registerRepository.Setup(r => r.CreateEpaOrganisationStandard(It.IsAny<EpaOrganisationStandard>()))
                .Returns(Task.FromResult(_expectedOrganisationStandardNoIssues.Id));

            _validator.Setup(v => v.CheckOrganisationIdIsPresentAndValid(_requestNoIssues.OrganisationId)).Returns(string.Empty);
            _validator.Setup(v => v.CheckIfOrganisationNotFound(_requestNoIssues.OrganisationId)).Returns(string.Empty);
            _validator.Setup(v => v.CheckIfStandardNotFound(_requestNoIssues.StandardCode)).Returns(string.Empty);
            _validator.Setup(v => v.CheckIfOrganisationStandardAlreadyExists(_requestNoIssues.OrganisationId, _requestNoIssues.StandardCode)).Returns(string.Empty);

            _createEpaOrganisationStandardHandler = new CreateEpaOrganisationStandardHandler(_registerRepository.Object, _validator.Object, _logger.Object);
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
        public void GetOrganisationDetailsWhenOrganisationStandardCreated()
        {
            var returnedId = _createEpaOrganisationStandardHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            returnedId.Should().Be(_expectedOrganisationStandardNoIssues.Id);
        }

        //[Test]
        //public void GetBadRequestExceptionWhenNoNameValidationOccurs()
        //{
        //    const string errorMessage = "No Name issue";
        //    var requestNoName = BuildRequest("", _organisationId, 123321);
        //    _validator.Setup(v => v.CheckOrganisationName(requestNoName.Name)).Returns(errorMessage);
        //    var ex = Assert.ThrowsAsync<BadRequestException>(() => _createEpaOrganisationHandler.Handle(requestNoName, new CancellationToken()));
        //    Assert.AreEqual(errorMessage, ex.Message);
        //    _registerRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);
        //    _validator.Verify(v => v.CheckOrganisationName(requestNoName.Name));
        //}

        //[Test]
        //public void GetBadRequestExceptionWhenNoOrganisationIdValidationOccurs()
        //{
        //    const string errorMessage = "No id issue";
        //    var requestNoOrgId = BuildRequest("name", "", 123321);
        //    _validator.Setup(v => v.CheckOrganisationIdIsPresentAndValid(requestNoOrgId.OrganisationId)).Returns(errorMessage);
        //    var ex = Assert.ThrowsAsync<BadRequestException>(() => _createEpaOrganisationHandler.Handle(requestNoOrgId, new CancellationToken()));
        //    Assert.AreEqual(errorMessage, ex.Message);
        //    _registerRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);
        //    _validator.Verify(v => v.CheckOrganisationIdIsPresentAndValid(requestNoOrgId.OrganisationId));
        //}

        //[Test]
        //public void GetBadRequestExceptionWhenOrganisationIdIssueValidationOccurs()
        //{
        //    const string errorMessage = "id issue";
        //    var requestNoOrgId = BuildRequest("name", "", 123321);
        //    _validator.Setup(v => v.CheckOrganisationIdIsPresentAndValid(requestNoOrgId.OrganisationId)).Returns(errorMessage);
        //    var ex = Assert.ThrowsAsync<BadRequestException>(() => _createEpaOrganisationHandler.Handle(requestNoOrgId, new CancellationToken()));
        //    Assert.AreEqual(errorMessage, ex.Message);
        //    _registerRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);
        //    _validator.Verify(v => v.CheckOrganisationIdIsPresentAndValid(requestNoOrgId.OrganisationId));
        //}

        //[Test]
        //public void GetBadRequestExceptionWhenUkprnInvalidFormatOccurs()
        //{
        //    const string errorMessage = "invalid ukprn";
        //    var requestInvalidUkprn = BuildRequest("name", "fdfsdf", 123321);
        //    _validator.Setup(v => v.CheckUkprnIsValid(requestInvalidUkprn.Ukprn)).Returns(errorMessage);
        //    var ex = Assert.ThrowsAsync<BadRequestException>(() => _createEpaOrganisationHandler.Handle(requestInvalidUkprn, new CancellationToken()));
        //    Assert.AreEqual(errorMessage, ex.Message);
        //    _registerRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);
        //    _validator.Verify(v => v.CheckUkprnIsValid(requestInvalidUkprn.Ukprn));
        //}


        //[Test]
        //public void GetBadRequestExceptionWhenOrganisationIdAlreadyExistsValidationOccurs()
        //{
        //    const string errorMessage = "id already exists";
        //    var requestNoOrgId = BuildRequest("name", "EPA888", 123321);
        //    _validator.Setup(v => v.CheckIfOrganisationAlreadyExists(requestNoOrgId.OrganisationId)).Returns(errorMessage);
        //    var ex = Assert.ThrowsAsync<AlreadyExistsException>(() => _createEpaOrganisationHandler.Handle(requestNoOrgId, new CancellationToken()));
        //    Assert.AreEqual(errorMessage, ex.Message);
        //    _registerRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);
        //    _validator.Verify(v => v.CheckIfOrganisationAlreadyExists(requestNoOrgId.OrganisationId));
        //}

        //[Test]
        //public void GetBadRequestExceptionWhenukprnAlreadyExistsValidationOccurs()
        //{
        //    const string errorMessage = "ukprn already exists";
        //    var requestNoUkprn = BuildRequest("name", "EPA888", 123321);
        //    _validator.Setup(v => v.CheckIfOrganisationUkprnExists(requestNoUkprn.Ukprn)).Returns(errorMessage);
        //    var ex = Assert.ThrowsAsync<AlreadyExistsException>(() => _createEpaOrganisationHandler.Handle(requestNoUkprn, new CancellationToken()));
        //    Assert.AreEqual(errorMessage, ex.Message);
        //    _registerRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);
        //    _validator.Verify(v => v.CheckIfOrganisationUkprnExists(requestNoUkprn.Ukprn));
        //}


        //[Test]
        //public void GetBadRequestExceptionWheOrganisationTypeIdValidationOccurs()
        //{
        //    const string errorMessage = "organisation type is invalid";
        //    var requestUnknownOrganisationTypeId = BuildRequest("name", "EPA888", 123321);
        //    _validator.Setup(v => v.CheckOrganisationTypeIsNullOrExists(requestUnknownOrganisationTypeId.OrganisationTypeId)).Returns(errorMessage);
        //    var ex = Assert.ThrowsAsync<BadRequestException>(() => _createEpaOrganisationHandler.Handle(requestUnknownOrganisationTypeId, new CancellationToken()));
        //    Assert.AreEqual(errorMessage, ex.Message);
        //    _registerRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);
        //    _validator.Verify(v => v.CheckOrganisationTypeIsNullOrExists(requestUnknownOrganisationTypeId.OrganisationTypeId));
        //}


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
                DateStandardApprovedOnRegister = request.DateStandardApprovedOnRegister,
                Comments = request.Comments,
                Status = request.Status
            };
        }
    }
}
