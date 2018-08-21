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
    public class RegisterCreateOrganisationHandlerTests
    {
        private Mock<IRegisterRepository> _registerRepository;
        private CreateEpaOrganisationHandler _createEpaOrganisationHandler;
        private string _returnedOrganisationId;
        private Mock<IEpaOrganisationValidator> _validator;
        private Mock<ILogger<CreateEpaOrganisationHandler>> _logger;
        private CreateEpaOrganisationRequest _requestNoIssues;
        private EpaOrganisation _expectedOrganisationNoIssues;
        private string _organisationId;

        [SetUp]
        public void Setup()
        {
            _registerRepository = new Mock<IRegisterRepository>();
            _validator = new Mock<IEpaOrganisationValidator>();
            _logger = new Mock<ILogger<CreateEpaOrganisationHandler>>();
            _organisationId = "EPA999";

            _requestNoIssues = BuildRequest("name 1",_organisationId,123321);
            _expectedOrganisationNoIssues = BuildOrganisation(_requestNoIssues);
     
            _registerRepository.Setup(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()))
                .Returns(Task.FromResult(_expectedOrganisationNoIssues.OrganisationId));

            _validator.Setup(v => v.CheckOrganisationIdIsPresentAndValid(_requestNoIssues.OrganisationId)).Returns(string.Empty);
            _validator.Setup(v => v.CheckOrganisationName(_requestNoIssues.Name)).Returns(string.Empty);
            _validator.Setup(v => v.CheckIfOrganisationAlreadyExists(_requestNoIssues.OrganisationId)).Returns(string.Empty);
            _validator.Setup(v => v.CheckIfOrganisationUkprnExists(_requestNoIssues.Ukprn)).Returns(string.Empty);
            _validator.Setup(v => v.CheckOrganisationTypeIsNullOrExists(_requestNoIssues.OrganisationTypeId)).Returns(string.Empty);
            _validator.Setup(v => v.CheckUkprnIsValid(_requestNoIssues.Ukprn)).Returns(string.Empty);

            _createEpaOrganisationHandler = new CreateEpaOrganisationHandler(_registerRepository.Object, _validator.Object,_logger.Object);
          }

        [Test]
        public void GetOrganisationDetailsRepoIsCalledWhenHandlerInvoked()
        {
            var res = _createEpaOrganisationHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            _registerRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()));
        }

        [Test]
        public void CheckAllValidatorsAreCalledWhenHandlerInvoked()
        {
           var res = _createEpaOrganisationHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            _validator.Verify(v => v.CheckOrganisationIdIsPresentAndValid(_requestNoIssues.OrganisationId));
            _validator.Verify(v => v.CheckOrganisationName(_requestNoIssues.Name));
            _validator.Verify(v => v.CheckIfOrganisationAlreadyExists(_requestNoIssues.OrganisationId));
            _validator.Verify(v => v.CheckIfOrganisationUkprnExists(_requestNoIssues.Ukprn));
            _validator.Verify(v => v.CheckOrganisationTypeIsNullOrExists(_requestNoIssues.OrganisationTypeId));
            _validator.Verify(v => v.CheckUkprnIsValid(_requestNoIssues.Ukprn));
        }

        [Test]
        public void GetOrganisationDetailsWhenOrganisationCreated()
        {
            _returnedOrganisationId = _createEpaOrganisationHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            _returnedOrganisationId.Should().BeEquivalentTo(_expectedOrganisationNoIssues.OrganisationId);
        }

        [Test]
        public void GetBadRequestExceptionWhenNoNameValidationOccurs()
        {
            const string errorMessage = "No Name issue";
            var requestNoName = BuildRequest("", _organisationId, 123321);
            _validator.Setup(v => v.CheckOrganisationName(requestNoName.Name)).Returns(errorMessage);
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _createEpaOrganisationHandler.Handle(requestNoName, new CancellationToken()));
            Assert.AreEqual(errorMessage,ex.Message);
            _registerRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()),Times.Never);
            _validator.Verify(v => v.CheckOrganisationName(requestNoName.Name));
        }

        [Test]
        public void GetBadRequestExceptionWhenNoOrganisationIdValidationOccurs()
        {
            const string errorMessage = "No id issue";
            var requestNoOrgId = BuildRequest("name", "", 123321);
            _validator.Setup(v => v.CheckOrganisationIdIsPresentAndValid(requestNoOrgId.OrganisationId)).Returns(errorMessage);
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _createEpaOrganisationHandler.Handle(requestNoOrgId, new CancellationToken()));
            Assert.AreEqual(errorMessage, ex.Message);
            _registerRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);
            _validator.Verify(v => v.CheckOrganisationIdIsPresentAndValid(requestNoOrgId.OrganisationId));
        }

        [Test]
        public void GetBadRequestExceptionWhenOrganisationIdIssueValidationOccurs()
        {
            const string errorMessage = "id issue";
            var requestNoOrgId = BuildRequest("name", "", 123321);
            _validator.Setup(v => v.CheckOrganisationIdIsPresentAndValid(requestNoOrgId.OrganisationId)).Returns(errorMessage);
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _createEpaOrganisationHandler.Handle(requestNoOrgId, new CancellationToken()));
            Assert.AreEqual(errorMessage, ex.Message);
            _registerRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);
            _validator.Verify(v => v.CheckOrganisationIdIsPresentAndValid(requestNoOrgId.OrganisationId));
        }

        [Test]
        public void GetBadRequestExceptionWhenUkprnInvalidFormatOccurs()
        {
            const string errorMessage = "invalid ukprn";
            var requestInvalidUkprn = BuildRequest("name", "fdfsdf", 123321);
            _validator.Setup(v => v.CheckUkprnIsValid(requestInvalidUkprn.Ukprn)).Returns(errorMessage);
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _createEpaOrganisationHandler.Handle(requestInvalidUkprn, new CancellationToken()));
            Assert.AreEqual(errorMessage, ex.Message);
            _registerRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);
            _validator.Verify(v => v.CheckUkprnIsValid(requestInvalidUkprn.Ukprn));
        }
        

        [Test]
        public void GetBadRequestExceptionWhenOrganisationIdAlreadyExistsValidationOccurs()
        {
            const string errorMessage = "id already exists";
            var requestNoOrgId = BuildRequest("name", "EPA888", 123321);
            _validator.Setup(v => v.CheckIfOrganisationAlreadyExists(requestNoOrgId.OrganisationId)).Returns(errorMessage);
            var ex = Assert.ThrowsAsync<AlreadyExistsException>(() => _createEpaOrganisationHandler.Handle(requestNoOrgId, new CancellationToken()));
            Assert.AreEqual(errorMessage, ex.Message);
            _registerRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);
            _validator.Verify(v => v.CheckIfOrganisationAlreadyExists(requestNoOrgId.OrganisationId));
        }

        [Test]
        public void GetBadRequestExceptionWhenukprnAlreadyExistsValidationOccurs()
        {
            const string errorMessage = "ukprn already exists";
            var requestNoUkprn = BuildRequest("name", "EPA888", 123321);
            _validator.Setup(v => v.CheckIfOrganisationUkprnExists(requestNoUkprn.Ukprn)).Returns(errorMessage);
            var ex = Assert.ThrowsAsync<AlreadyExistsException>(() => _createEpaOrganisationHandler.Handle(requestNoUkprn, new CancellationToken()));
            Assert.AreEqual(errorMessage, ex.Message);
            _registerRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);
            _validator.Verify(v => v.CheckIfOrganisationUkprnExists(requestNoUkprn.Ukprn));
        }


        [Test]
        public void GetBadRequestExceptionWheOrganisationTypeIdValidationOccurs()
        {
            const string errorMessage = "organisation type is invalid";
            var requestUnknownOrganisationTypeId = BuildRequest("name", "EPA888", 123321);
            _validator.Setup(v => v.CheckOrganisationTypeIsNullOrExists(requestUnknownOrganisationTypeId.OrganisationTypeId)).Returns(errorMessage);
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _createEpaOrganisationHandler.Handle(requestUnknownOrganisationTypeId, new CancellationToken()));
            Assert.AreEqual(errorMessage, ex.Message);
            _registerRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);
            _validator.Verify(v => v.CheckOrganisationTypeIsNullOrExists(requestUnknownOrganisationTypeId.OrganisationTypeId));
        }
        private CreateEpaOrganisationRequest BuildRequest(string name, string organisationId, long? ukprn)
        {
            return new CreateEpaOrganisationRequest
            {
                Name = name,
                OrganisationId = organisationId,
                Ukprn = ukprn,
                OrganisationTypeId = 5,
                LegalName = "legal name 1",
                WebsiteLink = "website link 1",
                Address1 = "address 1",
                Address2 = "address 2",
                Address3 = "address 3",
                Address4 = "address 4",
                Postcode = "postcode"
            };
        }

        private EpaOrganisation BuildOrganisation(CreateEpaOrganisationRequest request)
        {
            return new EpaOrganisation
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                Name = request.Name,
                OrganisationId = request.OrganisationId,
                Ukprn = request.Ukprn,
                PrimaryContact = null,
                Status = OrganisationStatus.New,
                OrganisationTypeId = request.OrganisationTypeId,
                OrganisationData = new OrganisationData
                {
                    LegalName = request.LegalName,
                    Address1 = request.Address1,
                    Address2 = request.Address2,
                    Address3 = request.Address3,
                    Address4 = request.Address4,
                    Postcode = request.Postcode
                }
            };
        }
    }
}
