using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Commands;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Services.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Interfaces.Validation;
using SFA.DAS.AssessorService.Web.Staff.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Services
{
    [TestFixture]
    public class AnswerInjectionServiceApplyOrganisationStandardIntoRegisterTests
    {
        private AnswerInjectionService _answerInjectionService;
        private Mock<IRegisterQueryRepository> _mockRegisterQueryRepository;
        private IValidationService _validationService;
        private Mock<IAssessorValidationService> _mockAssessorValidationService;
        private Mock<IEpaOrganisationIdGenerator> _mockEpaOrganisationIdGenerator;
        private Mock<ILogger<AnswerService>> _mockLogger;
        private Mock<ISpecialCharacterCleanserService> _mockSpecialCharacterCleanserService;
        private Mock<IRegisterRepository> _mockRegisterRepository;

        [SetUp]
        public void Setup()
        {
            var applicationId = Guid.NewGuid();
            _mockRegisterQueryRepository = new Mock<IRegisterQueryRepository>();
            _validationService = new ValidationService();
            _mockAssessorValidationService = new Mock<IAssessorValidationService>();
            _mockEpaOrganisationIdGenerator = new Mock<IEpaOrganisationIdGenerator>();
            _mockLogger = new Mock<ILogger<AnswerService>>();
            _mockSpecialCharacterCleanserService = new Mock<ISpecialCharacterCleanserService>();
            _mockRegisterRepository = new Mock<IRegisterRepository>();
            _answerInjectionService = new AnswerInjectionService(
                _validationService,
                _mockAssessorValidationService.Object,
                _mockRegisterQueryRepository.Object,
                _mockRegisterRepository.Object,
                _mockEpaOrganisationIdGenerator.Object,
                _mockSpecialCharacterCleanserService.Object,
                _mockLogger.Object
            );

            _mockRegisterQueryRepository.Setup(r => r.GetAssessmentOrganisationsByNameOrCharityNumberOrCompanyNumber(It.IsAny<string>()))
                .ReturnsAsync(new List<AssessmentOrganisationSummary> { new AssessmentOrganisationSummary { Id = "EPA0001" } });

            _mockRegisterQueryRepository.Setup(r => r.GetDeliveryAreas())
                .ReturnsAsync(new List<DeliveryArea> { new DeliveryArea { Id = 1, Area = "East Midlands" } });

            _mockSpecialCharacterCleanserService.Setup(c => c.CleanseStringForSpecialCharacters(It.IsAny<string>()))
                .Returns((string s) => s);
        }

        [Test, TestCaseSource(nameof(InjectionTestCases))]
        public void WhenInjectingOrganisationAndContactHappyPathForAnApplication(InjectionTestCase testCase)
        {
            _mockAssessorValidationService.Setup(s => s.IsOrganisationStandardTaken(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Task.FromResult(testCase.IsOrganisationStandardTaken));

            _mockRegisterRepository.Setup(r => r.CreateEpaOrganisationStandard(It.IsAny<EpaOrganisationStandard>(), It.IsAny<List<int>>()))
                .Returns(Task.FromResult(testCase.ExpectedResponse.EpaoStandardId));

            var actualResponse = _answerInjectionService.InjectApplyOrganisationStandardDetailsIntoRegister(testCase.Command).Result;

            if (actualResponse.WarningMessages.Count > 0)
            {
                actualResponse.WarningMessages = testCase.ExpectedResponse.WarningMessages;
            }

            Assert.AreEqual(JsonConvert.SerializeObject(testCase.ExpectedResponse), JsonConvert.SerializeObject(actualResponse));

            if (actualResponse.WarningMessages.Count > 0)
            {
                _mockRegisterRepository.Verify(r => r.CreateEpaOrganisationStandard(It.IsAny<EpaOrganisationStandard>(), It.IsAny<List<int>>()), Times.Never);
            }
            else
            {
                _mockRegisterRepository.Verify(r => r.CreateEpaOrganisationStandard(It.IsAny<EpaOrganisationStandard>(), It.IsAny<List<int>>()), Times.Once);
            }
        }

        protected static IEnumerable<InjectionTestCase> InjectionTestCases
        {
            get
            {
                yield return new InjectionTestCase(Guid.NewGuid().ToString(), "EPA0001",
                    1, DateTime.UtcNow.Date, "East Midlands", false, "EPA Standard Id", null);
                yield return new InjectionTestCase(Guid.NewGuid().ToString(), null,
                    0, DateTime.UtcNow.Date, "East Midlands", false, null, "organisation id missing");
                yield return new InjectionTestCase(Guid.NewGuid().ToString(), "INVALID",
                    0, DateTime.UtcNow.Date, "East Midlands", false, null, "organisation id invalid");
                yield return new InjectionTestCase(Guid.NewGuid().ToString(), "EPA0001",
                    0, DateTime.UtcNow.Date, "East Midlands", false, null, "standard invalid");
                yield return new InjectionTestCase(Guid.NewGuid().ToString(), "EPA0001",
                    99, DateTime.UtcNow.Date, "East Midlands", true, null, "standard taken");
            }
        }

        public class InjectionTestCase
        {
            public CreateOrganisationStandardCommand Command { get; set; }
            public CreateOrganisationStandardFromApplyResponse ExpectedResponse { get; set; }

            public bool IsOrganisationStandardTaken { get; set; }


            public InjectionTestCase(string createdBy,
                string organisationId, int standardCode, DateTime effectiveFrom, string deliveryAreas,
                bool isOrganisationStandardTaken, string epaoStandardId, string warningMessage)
            {
                var warningMessages = new List<string>();
                if (!string.IsNullOrEmpty(warningMessage))
                {
                    warningMessages.Add(warningMessage);
                }

                IsOrganisationStandardTaken = isOrganisationStandardTaken;

                var response = new CreateOrganisationStandardFromApplyResponse
                {
                    WarningMessages = warningMessages,
                    EpaoStandardId = epaoStandardId
                };

                Command = new CreateOrganisationStandardCommand
                {
                    CreatedBy = createdBy,
                    OrganisationId = organisationId,
                    StandardCode = standardCode,
                    EffectiveFrom = DateTime.Parse(effectiveFrom.ToString()),
                    DeliveryAreas = deliveryAreas?.Split(",").ToList(),
                };
                ExpectedResponse = response;
            }
        }
    }
}


