using System;
using System.Runtime.CompilerServices;
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
    public class RegisterUpdateOrganisationHandlerTests
    {
        private Mock<IRegisterRepository> _registerRepository;
        private UpdateEpaOrganisationHandler _updateEpaOrganisationHandler;
        private string _returnedOrganisationId;
        private Mock<ISpecialCharacterCleanserService> _cleanserService;
        private Mock<ILogger<UpdateEpaOrganisationHandler>> _logger;
        private Mock<IEpaOrganisationValidator> _validator;
        private UpdateEpaOrganisationRequest _requestNoIssues;
        private EpaOrganisation _expectedOrganisationNoIssues;
        private string _organisationId;

        [SetUp]
        public void Setup()
        {
            _registerRepository = new Mock<IRegisterRepository>();
            _cleanserService = new Mock<ISpecialCharacterCleanserService>();
            _validator = new Mock<IEpaOrganisationValidator>();
            _logger = new Mock<ILogger<UpdateEpaOrganisationHandler>>();
            _organisationId = "EPA999";

            _requestNoIssues = BuildRequest("name 1", _organisationId, 123321);
            _expectedOrganisationNoIssues = BuildOrganisation(_requestNoIssues);

            _registerRepository.Setup(r => r.UpdateEpaOrganisation(It.IsAny<EpaOrganisation>()))
                .Returns(Task.FromResult(_expectedOrganisationNoIssues.OrganisationId));

            _cleanserService.Setup(c => c.CleanseStringForSpecialCharacters(It.IsAny<string>()))
                .Returns((string s) => s);
            
            _updateEpaOrganisationHandler = new UpdateEpaOrganisationHandler(_registerRepository.Object, _logger.Object, _cleanserService.Object, _validator.Object);
        }

        [Test]
        public void GetOrganisationDetailsRepoIsCalledWhenHandlerInvoked()
        {
            var validationResponse = new ValidationResponse();

            _validator.Setup(x => x.ValidatorUpdateEpaOrganisationRequest(It.IsAny<UpdateEpaOrganisationRequest>()))
                .Returns(validationResponse);

            var res = _updateEpaOrganisationHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            _registerRepository.Verify(r => r.UpdateEpaOrganisation(It.IsAny<EpaOrganisation>()));
        }

        [Test]
        public void GetOrganisationDetailsWhenOrganisationUpdated()
        {
            var validationResponse = new ValidationResponse();

            _validator.Setup(x => x.ValidatorUpdateEpaOrganisationRequest(It.IsAny<UpdateEpaOrganisationRequest>()))
                .Returns(validationResponse);

            _returnedOrganisationId = _updateEpaOrganisationHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            _returnedOrganisationId.Should().BeEquivalentTo(_expectedOrganisationNoIssues.OrganisationId);
        }

        [Test]
        public void UpdateOrganisationFailsValidation()
        {
            UpdateEpaOrganisationRequest request = BuildRequest("Name", "OrgId", 10001122);

            var validationResponse = BuildErrorResponse("Message", ValidationStatusCode.BadRequest);

            _validator.Setup(x => x.ValidatorUpdateEpaOrganisationRequest(It.IsAny<UpdateEpaOrganisationRequest>()))
                .Returns(validationResponse);

            Task<string> updateOperation = _updateEpaOrganisationHandler.Handle(request, new CancellationToken());
            string message;
            _registerRepository.Verify(r => r.UpdateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);

            Assert.Throws<AggregateException>(() => message = updateOperation.Result);
        }

        private ValidationResponse BuildErrorResponse(string errorMessage, ValidationStatusCode statusCode)
        {
            var validationResponse = new ValidationResponse();
            validationResponse.Errors.Add(new ValidationErrorDetail(errorMessage,statusCode));
            return validationResponse;
        }

        private UpdateEpaOrganisationRequest BuildRequest(string name, string organisationId, long? ukprn)
        {
            return new UpdateEpaOrganisationRequest
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
                Postcode = "postcode",
                CompanyNumber = "12345678",
                CharityNumber = "1234567"
            };
        }

        private EpaOrganisation BuildOrganisation(UpdateEpaOrganisationRequest request)
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
                    Postcode = request.Postcode,
                    CompanyNumber = request.CompanyNumber,
                    CharityNumber = request.CharityNumber
                }
            };
        }
    }
}
