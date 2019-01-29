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
        private Mock<ILogger<CreateEpaOrganisationHandler>> _logger;
        private Mock<IEpaOrganisationIdGenerator> _idGenerator;
        private Mock<IEpaOrganisationValidator> _validator;
        private CreateEpaOrganisationRequest _requestNoIssues;
        private EpaOrganisation _expectedOrganisationNoIssues;
        private string _organisationId;

        [SetUp]
        public void Setup()
        {
            _registerRepository = new Mock<IRegisterRepository>();
            _cleanserService = new Mock<ISpecialCharacterCleanserService>();
            _logger = new Mock<ILogger<CreateEpaOrganisationHandler>>();
            _idGenerator = new Mock<IEpaOrganisationIdGenerator>();
            _validator = new Mock<IEpaOrganisationValidator>();
            _organisationId = "EPA999";

            _requestNoIssues = BuildRequest("name 1",123321);
            _expectedOrganisationNoIssues = BuildOrganisation(_requestNoIssues, _organisationId);
     
            _registerRepository.Setup(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()))
                .Returns(Task.FromResult(_expectedOrganisationNoIssues.OrganisationId));

            _cleanserService.Setup(c => c.CleanseStringForSpecialCharacters(It.IsAny<string>()))
                .Returns((string s) => s);

            var validationResponse = new ValidationResponse();
            _validator.Setup(x => x.ValidatorCreateEpaOrganisationRequest(It.IsAny<CreateEpaOrganisationRequest>()))
                .Returns(validationResponse);

            _createEpaOrganisationHandler = new CreateEpaOrganisationHandler(_registerRepository.Object, _idGenerator.Object,_logger.Object, _cleanserService.Object, _validator.Object);
          }

        [Test]
        public void GetOrganisationDetailsRepoIsCalledWhenHandlerInvoked()
        {
            var result = _createEpaOrganisationHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            _registerRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()));
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
        public void CreateOrganisationFailsValidation()
        {
            var validationResponse = BuildErrorResponse("Message", ValidationStatusCode.BadRequest);
            _validator.Setup(x => x.ValidatorCreateEpaOrganisationRequest(It.IsAny<CreateEpaOrganisationRequest>()))
                .Returns(validationResponse);

            CreateEpaOrganisationRequest request = BuildRequest("Name", 100001234);

            Task<string> createOperation = _createEpaOrganisationHandler.Handle(request, new CancellationToken());
            string message;
            _registerRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);

            Assert.Throws<AggregateException>(() => message = createOperation.Result);
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
                Postcode = "postcode",
                CompanyNumber = "12345678",
                CharityNumber = "12345678"
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
                    Postcode = request.Postcode,
                    CompanyNumber = request.CompanyNumber,
                    CharityNumber = request.CharityNumber
                }
            };
        }

        private ValidationResponse BuildErrorResponse(string errorMessage, ValidationStatusCode statusCode)
        {
            var validationResponse = new ValidationResponse();
            validationResponse.Errors.Add(new ValidationErrorDetail(errorMessage, statusCode));
            return validationResponse;
        }
    }
}
