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

            _validator.Setup(v => v.ValidatorCreateEpaOrganisationStandardRequest(_requestNoIssues)).Returns(new ValidationResponse()); ;
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
            _validator.Verify(v => v.ValidatorCreateEpaOrganisationStandardRequest(_requestNoIssues));
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
            var errorResponse = BuildErrorResponse(errorMessage,  ValidationStatusCode.BadRequest);
            _validator.Setup(v => v.ValidatorCreateEpaOrganisationStandardRequest(requestNoOrgId)).Returns(errorResponse);
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _createEpaOrganisationStandardHandler.Handle(requestNoOrgId, new CancellationToken()));
            Assert.AreEqual(errorMessage + "; ", ex.Message);
            _registerRepository.Verify(r => r.CreateEpaOrganisationStandard(It.IsAny<EpaOrganisationStandard>()), Times.Never);
            _validator.Verify(v => v.ValidatorCreateEpaOrganisationStandardRequest(requestNoOrgId));
        }
        
        [Test]
        public void GetNotFoundExceptionWhenOrganisationIdIssueValidationOccurs()
        {
            const string errorMessage = "no organisation Id";
            var requestOrgNotFound = BuildRequest("", 1);
            var errorResponse = BuildErrorResponse(errorMessage,  ValidationStatusCode.NotFound);
            _validator.Setup(v => v.ValidatorCreateEpaOrganisationStandardRequest(requestOrgNotFound)).Returns(errorResponse);
            var ex = Assert.ThrowsAsync<NotFound>(() => _createEpaOrganisationStandardHandler.Handle(requestOrgNotFound, new CancellationToken()));
            Assert.AreEqual(errorMessage + "; ", ex.Message);
            _registerRepository.Verify(r => r.CreateEpaOrganisationStandard(It.IsAny<EpaOrganisationStandard>()), Times.Never);
            _validator.Verify(v => v.ValidatorCreateEpaOrganisationStandardRequest(requestOrgNotFound));
        }
        
        [Test]
        public void GetAlreadyExistsExceptionWhenOrganisationIdIssueValidationOccurs()
        {
            const string errorMessage = "organisation/contact already exists";
            var requestNoOrgId = BuildRequest("", 1);
            var errorResponse = BuildErrorResponse(errorMessage,  ValidationStatusCode.AlreadyExists);
            _validator.Setup(v => v.ValidatorCreateEpaOrganisationStandardRequest(requestNoOrgId)).Returns(errorResponse);
            var ex = Assert.ThrowsAsync<AlreadyExistsException>(() => _createEpaOrganisationStandardHandler.Handle(requestNoOrgId, new CancellationToken()));
            Assert.AreEqual(errorMessage + "; ", ex.Message);
            _registerRepository.Verify(r => r.CreateEpaOrganisationStandard(It.IsAny<EpaOrganisationStandard>()), Times.Never);
            _validator.Verify(v => v.ValidatorCreateEpaOrganisationStandardRequest(requestNoOrgId));
        }       
        
        private ValidationResponse BuildErrorResponse(string errorMessage, ValidationStatusCode statusCode)
        {
            var validationResponse = new ValidationResponse();
            validationResponse.Errors.Add(new ValidationErrorDetail(errorMessage,statusCode));
            return validationResponse;
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
