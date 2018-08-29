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
        private Mock<IEpaOrganisationIdGenerator> _idGenerator;
        private CreateEpaOrganisationRequest _requestNoIssues;
        private EpaOrganisation _expectedOrganisationNoIssues;
        private string _organisationId;

        [SetUp]
        public void Setup()
        {
            _registerRepository = new Mock<IRegisterRepository>();
            _validator = new Mock<IEpaOrganisationValidator>();
            _logger = new Mock<ILogger<CreateEpaOrganisationHandler>>();
            _idGenerator = new Mock<IEpaOrganisationIdGenerator>();
            _organisationId = "EPA999";

            _requestNoIssues = BuildRequest("name 1",123321);
            _expectedOrganisationNoIssues = BuildOrganisation(_requestNoIssues, _organisationId);
     
            _registerRepository.Setup(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()))
                .Returns(Task.FromResult(_expectedOrganisationNoIssues.OrganisationId));

            _validator.Setup(v => v.CheckOrganisationName(_requestNoIssues.Name)).Returns(string.Empty);
            _validator.Setup(v => v.CheckIfOrganisationUkprnExists(_requestNoIssues.Ukprn)).Returns(string.Empty);
            _validator.Setup(v => v.CheckOrganisationTypeIsNullOrExists(_requestNoIssues.OrganisationTypeId)).Returns(string.Empty);
            _validator.Setup(v => v.CheckUkprnIsValid(_requestNoIssues.Ukprn)).Returns(string.Empty);

            _createEpaOrganisationHandler = new CreateEpaOrganisationHandler(_registerRepository.Object, _validator.Object, _idGenerator.Object,_logger.Object);
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
            _validator.Verify(v => v.CheckOrganisationName(_requestNoIssues.Name));
            _validator.Verify(v => v.CheckIfOrganisationUkprnExists(_requestNoIssues.Ukprn));
            _validator.Verify(v => v.CheckOrganisationTypeIsNullOrExists(_requestNoIssues.OrganisationTypeId));
            _validator.Verify(v => v.CheckUkprnIsValid(_requestNoIssues.Ukprn));
        }

        [Test]
        public void CheckOrganisationIdGeneratorIsCalledWhenHandlerInvoked()
        {
            var res = _createEpaOrganisationHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            _idGenerator.Verify(g => g.GetNextOrganisationId());
        }

        [Test]
        public void GetOrganisationDetailsWhenOrganisationCreated()
        {
            _returnedOrganisationId = _createEpaOrganisationHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            _returnedOrganisationId.Should().BeEquivalentTo(_expectedOrganisationNoIssues.OrganisationId);
        }

        [Test]
        public void GetExceptionWhenNoValidOrganisationIdIsGenerated()
        {
            const string errorMessage = "A valid organisation Id could not be generated";
            var requestWithNoIdGenerated = BuildRequest("org without id coming", 123322);
            _idGenerator.Setup(g => g.GetNextOrganisationId()).Returns(string.Empty);
            var ex = Assert.ThrowsAsync<Exception>(() => _createEpaOrganisationHandler.Handle(requestWithNoIdGenerated, new CancellationToken()));
            Assert.AreEqual(errorMessage, ex.Message);
            _registerRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);
           _idGenerator.Verify(v => v.GetNextOrganisationId());
        }

        [Test]
        public void GetBadRequestExceptionWhenNoNameValidationOccurs()
        {
            const string errorMessage = "No Name issue";
            var requestNoName = BuildRequest("", 123321);
            _validator.Setup(v => v.CheckOrganisationName(requestNoName.Name)).Returns(errorMessage);
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _createEpaOrganisationHandler.Handle(requestNoName, new CancellationToken()));
            Assert.AreEqual(errorMessage,ex.Message);
            _registerRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()),Times.Never);
            _validator.Verify(v => v.CheckOrganisationName(requestNoName.Name));
        }  

        [Test]
        public void GetBadRequestExceptionWhenUkprnInvalidFormatOccurs()
        {
            const string errorMessage = "invalid ukprn";
            var requestInvalidUkprn = BuildRequest("name", 123321);
            _validator.Setup(v => v.CheckUkprnIsValid(requestInvalidUkprn.Ukprn)).Returns(errorMessage);
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _createEpaOrganisationHandler.Handle(requestInvalidUkprn, new CancellationToken()));
            Assert.AreEqual(errorMessage, ex.Message);
            _registerRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);
            _validator.Verify(v => v.CheckUkprnIsValid(requestInvalidUkprn.Ukprn));
        }
    
        [Test]
        public void GetBadRequestExceptionWhenukprnAlreadyExistsValidationOccurs()
        {
            const string errorMessage = "ukprn already exists";
            var requestNoUkprn = BuildRequest("name", 123321);
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
            var requestUnknownOrganisationTypeId = BuildRequest("name", 123321);
            _validator.Setup(v => v.CheckOrganisationTypeIsNullOrExists(requestUnknownOrganisationTypeId.OrganisationTypeId)).Returns(errorMessage);
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _createEpaOrganisationHandler.Handle(requestUnknownOrganisationTypeId, new CancellationToken()));
            Assert.AreEqual(errorMessage, ex.Message);
            _registerRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);
            _validator.Verify(v => v.CheckOrganisationTypeIsNullOrExists(requestUnknownOrganisationTypeId.OrganisationTypeId));
        }
        private CreateEpaOrganisationRequest BuildRequest(string name, long? ukprn)
        {
            return new CreateEpaOrganisationRequest
            {
                Name = name,
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

        private EpaOrganisation BuildOrganisation(CreateEpaOrganisationRequest request, string organisationId)
        {
            return new EpaOrganisation
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                Name = request.Name,
                OrganisationId = organisationId,
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
