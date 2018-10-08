using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
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
        private Mock<ISpecialCharacterCleanserService> _cleanserService;
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
            _cleanserService = new Mock<ISpecialCharacterCleanserService>();
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
            _validator.Setup(v => v.ValidatorCreateEpaOrganisationRequest(_requestNoIssues)).Returns(new ValidationResponse());
            _cleanserService.Setup(c => c.CleanseStringForSpecialCharacters(It.IsAny<string>()))
                .Returns((string s) => s);
            _createEpaOrganisationHandler = new CreateEpaOrganisationHandler(_registerRepository.Object, _validator.Object, _idGenerator.Object,_logger.Object, _cleanserService.Object);
          }

        [Test]
        public void GetOrganisationDetailsRepoIsCalledWhenHandlerInvoked()
        {
            var result = _createEpaOrganisationHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            _registerRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()));
        }

        [Test]
        public void CheckMainValidatorIsCalledWhenHandlerInvoked()
        {
           var result = _createEpaOrganisationHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            _validator.Verify(v => v.ValidatorCreateEpaOrganisationRequest(_requestNoIssues));
        }

        [Test]
        public void CheckOrganisationIdGeneratorIsCalledWhenHandlerInvoked()
        {
            var result = _createEpaOrganisationHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            _idGenerator.Verify(g => g.GetNextOrganisationId());
        }

        [Test]
        public void GetOrganisationDetailsWhenOrganisationCreated()
        {
            _returnedOrganisationId = _createEpaOrganisationHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            _returnedOrganisationId.Should().BeEquivalentTo(_expectedOrganisationNoIssues.OrganisationId);
        }
        
        [Test]
        public void GeExceptionWheValidationOccurs()
        {
            const string errorMessage = "organisation type is invalid";
            var requestUnknownOrganisationTypeId = BuildRequest("name", 123321);
            var errorResponse = BuildErrorResponse(errorMessage,  ValidationStatusCode.BadRequest);
            _validator.Setup(v => v.ValidatorCreateEpaOrganisationRequest(requestUnknownOrganisationTypeId)).Returns(errorResponse);
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _createEpaOrganisationHandler.Handle(requestUnknownOrganisationTypeId, new CancellationToken()));
            Assert.AreEqual(errorMessage + "; ", ex.Message);
            _registerRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);
            _validator.Verify(v => v.ValidatorCreateEpaOrganisationRequest(requestUnknownOrganisationTypeId));
        }

        private ValidationResponse BuildErrorResponse(string errorMessage, ValidationStatusCode statusCode)
        {
            var validationResponse = new ValidationResponse();
            validationResponse.Errors.Add(new ValidationErrorDetail(errorMessage,statusCode));
            return validationResponse;
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
