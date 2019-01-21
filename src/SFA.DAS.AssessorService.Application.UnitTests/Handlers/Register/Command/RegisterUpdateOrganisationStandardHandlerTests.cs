using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Register.Command
{
        [TestFixture]
        public class RegisterUpdateOrganisationStandardHandlerTests
        {
            private Mock<IRegisterRepository> _registerRepository;
            private UpdateEpaOrganisationStandardHandler _updateEpaOrganisationStandardHandler;
            private Mock<ISpecialCharacterCleanserService> _cleanserService;
            private Mock<IEpaOrganisationValidator> _validator;
            private Mock<ILogger<UpdateEpaOrganisationStandardHandler>> _logger;
            private UpdateEpaOrganisationStandardRequest _requestNoIssues;
            private string _organisationId;
            private EpaOrganisationStandard _expectedOrganisationStandardNoIssues;
            private int _requestNoIssuesId;


            [SetUp]
            public void Setup()
            {
                _registerRepository = new Mock<IRegisterRepository>();
                _cleanserService = new Mock<ISpecialCharacterCleanserService>();
                _validator = new Mock<IEpaOrganisationValidator>();
                _logger = new Mock<ILogger<UpdateEpaOrganisationStandardHandler>>();
                _organisationId = "EPA999";
                _requestNoIssuesId = 1;

                _requestNoIssues = BuildRequest(_organisationId, 123321, new List<int> { 1 });
                _expectedOrganisationStandardNoIssues = BuildOrganisationStandard(_requestNoIssues, _requestNoIssuesId);

                _registerRepository.Setup(r => r.UpdateEpaOrganisationStandard(It.IsAny<EpaOrganisationStandard>(), new List<int>{1}, It.IsAny<string>()))
                    .Returns(Task.FromResult(_expectedOrganisationStandardNoIssues.Id.ToString()));

                _validator.Setup(v => v.ValidatorUpdateEpaOrganisationStandardRequest(_requestNoIssues)).Returns(new ValidationResponse());
                _cleanserService.Setup(c => c.CleanseStringForSpecialCharacters(It.IsAny<string>()))
                    .Returns((string s) => s);
                
                _updateEpaOrganisationStandardHandler = new UpdateEpaOrganisationStandardHandler(_registerRepository.Object, _validator.Object, _logger.Object, _cleanserService.Object);
            }


            [Test]
            public void UpdateOrganisationStandardDetailsRepoIsCalledWhenHandlerInvoked()
            {
                var res = _updateEpaOrganisationStandardHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
                _registerRepository.Verify(r => r.UpdateEpaOrganisationStandard(It.IsAny<EpaOrganisationStandard>(), It.IsAny<List<int>>(), It.IsAny<string>()));
            }

            [Test]
            public void CheckValidatorIsCalledWhenHandlerInvoked()
            {
                var res = _updateEpaOrganisationStandardHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
                _validator.Verify(v => v.ValidatorUpdateEpaOrganisationStandardRequest(_requestNoIssues));
            }

            [Test]
            public void GetOrganisationStandardWhenOrganisationStandardUpdated()
            {
                var returnedId = _updateEpaOrganisationStandardHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
                returnedId.Should().Be(_expectedOrganisationStandardNoIssues.Id.ToString());
            }

            [Test]
            public void GetBadRequestExceptionWhenOrganisationIdStandardCodeDoesNotExist()
            {
                const string errorMessage = "no organisation Id";
                var requestNoOrgId = BuildRequest("org 1", 1, new List<int> { 1 });
                var errorResponse = BuildErrorResponse(errorMessage,  ValidationStatusCode.BadRequest);
                _validator.Setup(v => v.ValidatorUpdateEpaOrganisationStandardRequest(requestNoOrgId)).Returns(errorResponse);
                  var ex = Assert.ThrowsAsync<BadRequestException>(() => _updateEpaOrganisationStandardHandler.Handle(requestNoOrgId, new CancellationToken()));
                Assert.AreEqual(errorMessage + "; ", ex.Message);
                _registerRepository.Verify(r => r.UpdateEpaOrganisationStandard(It.IsAny<EpaOrganisationStandard>(), new List<int>(), "Save"), Times.Never);
                _validator.Verify(v => v.ValidatorUpdateEpaOrganisationStandardRequest(requestNoOrgId));
            }

        private UpdateEpaOrganisationStandardRequest BuildRequest(string organisationId, int standardCode, List<int> deliveryAreas)
            {
                return new UpdateEpaOrganisationStandardRequest
                {
                    OrganisationId = organisationId,
                    StandardCode = standardCode,
                    EffectiveFrom = null,
                    DeliveryAreas = deliveryAreas
                };
            }

        private ValidationResponse BuildErrorResponse(string errorMessage, ValidationStatusCode statusCode)
            {
                var validationResponse = new ValidationResponse();
                validationResponse.Errors.Add(new ValidationErrorDetail(errorMessage,statusCode));
                return validationResponse;
            }

        private EpaOrganisationStandard BuildOrganisationStandard(UpdateEpaOrganisationStandardRequest request, int id)
            {
                return new EpaOrganisationStandard
                {
                    Id = id,
                    OrganisationId = request.OrganisationId,
                    StandardCode = request.StandardCode,
                    EffectiveFrom = request.EffectiveFrom,
                    EffectiveTo = request.EffectiveTo,
                    Comments = request.Comments,
                    Status = "New"
                };
            }
    }
}
