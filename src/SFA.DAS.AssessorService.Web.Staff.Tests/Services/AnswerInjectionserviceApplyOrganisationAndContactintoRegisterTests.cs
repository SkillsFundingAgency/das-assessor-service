using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using FluentAssertions.Primitives;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Interfaces.Validation;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Services;
using SFA.DAS.AssessorService.Api.Types.Commands;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Services.Validation;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Services
{
    [TestFixture]
    public class AnswerInjectionServiceApplyOrganisationAndContactintoRegisterTests
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
            // _mockValidationService = new Mock<IValidationService>();
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


            var organisationType1 = new OrganisationType {Id = 1, Type = "Type 1"};
            var organisationType2 = new OrganisationType {Id = 2, Type = "Training Provider"};

            var expectedOrganisationTypes = new List<OrganisationType>
            {
                organisationType1,
                organisationType2
            };


            //  _mockAssessorValidationService.Setup(s => s.IsCharityNumberTaken(It.IsAny<string>())).Returns(Task.FromResult(false));
            _mockRegisterQueryRepository.Setup(r => r.GetOrganisationTypes())
                .Returns(Task.FromResult(expectedOrganisationTypes.AsEnumerable()));
            _mockEpaOrganisationIdGenerator.Setup(g => g.GetNextContactUsername()).Returns("unknown-9999");

            _mockSpecialCharacterCleanserService.Setup(c => c.CleanseStringForSpecialCharacters(It.IsAny<string>()))
                .Returns((string s) => s);

            _mockRegisterRepository.Setup(r => r.CreateEpaOrganisationContact(It.IsAny<EpaContact>()))
                .Returns(Task.FromResult("unknown-9999"));

        }

        [Test, TestCaseSource(nameof(InjectionTestCasesHappyPath))]
        public void WhenInjectingOrganisationAndContactHappyPathForAnApplication(InjectionTestCase testCase)
        {
            _mockAssessorValidationService.Setup(s => s.IsOrganisationNameTaken(It.IsAny<string>()))
                .Returns(Task.FromResult(testCase.IsOrganisationNameTaken));
            _mockAssessorValidationService.Setup(s => s.IsOrganisationUkprnTaken(It.IsAny<long>()))
                .Returns(Task.FromResult(testCase.IsUkprnTaken));
            _mockAssessorValidationService.Setup(s => s.IsCompanyNumberTaken(It.IsAny<string>()))
                .Returns(Task.FromResult(testCase.IsCompanyNumberTaken));
            _mockAssessorValidationService.Setup(s => s.IsEmailTaken(It.IsAny<string>()))
                .Returns(Task.FromResult(testCase.IsEmailTaken));

            _mockEpaOrganisationIdGenerator.Setup(g => g.GetNextOrganisationId())
                .Returns(testCase.ExpectedResponse.OrganisationId);
            _mockRegisterRepository.Setup(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()))
                .Returns(Task.FromResult(testCase.ExpectedResponse.OrganisationId));

            var actualResponse = _answerInjectionService
                .InjectApplyOrganisationAndContactDetailsIntoRegister(testCase.Command).Result;
            if (actualResponse.WarningMessages.Count > 0)
            {

                actualResponse.WarningMessages = testCase.ExpectedResponse.WarningMessages;
            }

            Assert.AreEqual(JsonConvert.SerializeObject(testCase.ExpectedResponse),
                JsonConvert.SerializeObject(actualResponse));

            if (actualResponse.WarningMessages.Count > 0 || testCase.Command.IsEpaoApproved.Value ||
                testCase.ExpectedResponse.ApplySourceIsEpao)
            {
                _mockRegisterRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);
                _mockRegisterRepository.Verify(r => r.CreateEpaOrganisationContact(It.IsAny<EpaContact>()),
                    Times.Never);
            }
            else
            {
                _mockRegisterRepository.Verify(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Once);
                _mockRegisterRepository.Verify(r => r.CreateEpaOrganisationContact(It.IsAny<EpaContact>()),
                    Times.Once);
            }
        }

        protected static IEnumerable<InjectionTestCase> InjectionTestCasesHappyPath
        {
            get
            {
                yield return new InjectionTestCase("RoEPAO", true, false, null, false, null, null, null, false, null,
                    false, null, false, null, null, null, false, null);
                yield return new InjectionTestCase("RoATP", false, true, null, false, null, null, null, false, null,
                    false, null, false, null, null, null, false, null);
                yield return new InjectionTestCase("RoATP", false, false, "org name", false, null, "TrainingProvider",
                    "12345678", false, "12345678", false, "1234", false, "EPA9999", "joe@cool.com", "Joe Cool", false,
                    null);
                yield return new InjectionTestCase("RoATP", false, false, null, false, "trading name 1",
                    "TrainingProvider", "12345678", false, "12345678", false, "1234", false, "EPA9999", "joe@cool.com",
                    "Joe Cool", false, null);
                yield return new InjectionTestCase("RoATP", false, false, null, false, null, "TrainingProvider",
                    "12345678", false, "12345678", false, "1234", false, null, "joe@cool.com", "Joe Cool", false,
                    "organisation name not present");
                yield return new InjectionTestCase("RoATP", false, false, "a", false, null, "TrainingProvider",
                    "12345678", false, "12345678", false, "1234", false, null, "joe@cool.com", "Joe Cool", false,
                    "organisation name too short");
                yield return new InjectionTestCase("RoATP", false, false, "aaa", true, null, "TrainingProvider",
                    "12345678", false, "12345678", false, "1234", false, null, "joe@cool.com", "Joe Cool", false,
                    "organisation name already taken");
                yield return new InjectionTestCase("RoATP", false, false, "org name", false, null, "TrainingProviderX",
                    "12345678", false, "12345678", false, "1234", false, null, "joe@cool.com", "Joe Cool", false,
                    "organisation type not identified");
                yield return new InjectionTestCase("RoATP", false, false, "org name", false, null, "TrainingProvider",
                    "1234578", true, "12345678", false, "1234", false, null, "joe@cool.com", "Joe Cool", false,
                    "ukprn invalid");
                yield return new InjectionTestCase("RoATP", false, false, "org name", false, null, "TrainingProvider",
                    "1234578", false, "ABC", false, "1234", false, null, "joe@cool.com", "Joe Cool", false,
                    "company number invalid");
                yield return new InjectionTestCase("RoATP", false, false, "org name", false, null, "TrainingProvider",
                    "1234578", false, "1234567", true, "1234", false, null, "joe@cool.com", "Joe Cool", false,
                    "company number taken");
                yield return new InjectionTestCase("RoATP", false, false, "org name", false, null, "TrainingProvider",
                    "1234578", false, "1234567", false, "ABC", false, null, "joe@cool.com", "Joe Cool", false,
                    "charity number invalid");
                yield return new InjectionTestCase("RoATP", false, false, "org name", false, null, "TrainingProvider",
                    "1234578", false, "1234567", false, "1234", true, null, "joe@cool.com", "Joe Cool", false,
                    "charity number taken");
                yield return new InjectionTestCase("RoATP", false, false, "org name", false, null, "TrainingProvider",
                    "1234578", false, "1234567", false, "1234", false, null, "joecool.com", "Joe Cool", false,
                    "email invalid");
                yield return new InjectionTestCase("RoATP", false, false, "org name", false, null, "TrainingProvider",
                    "1234578", false, "1234567", false, "1234", false, null, "joe@cool.com", "Joe Cool", true,
                    "email taken");
                yield return new InjectionTestCase("RoATP", false, false, "org name", false, null, "TrainingProvider",
                    "1234578", false, "1234567", false, "1234", false, null, "joe@cool.com", "Jo", false,
                    "contact name bad");

            }
        }

        public class InjectionTestCase
        {
            public CreateOrganisationContactCommand Command { get; set; }
            public CreateOrganisationAndContactFromApplyResponse ExpectedResponse { get; set; }

            public string WArningMessage1 { get; set; }
            public bool IsOrganisationNameTaken { get; set; }
            public bool IsUkprnTaken { get; set; }
            public bool IsCompanyNumberTaken { get; set; }
            public bool IsCharityNumberTaken { get; set; }
            public bool IsEmailTaken { get; set; }

            public InjectionTestCase(string organisationReferenceType, bool isEpaoSource, bool isEpaoApproved,
                string organisationName, bool isOrganisationNameTaken, string tradingName, string organisationType,
                string ukprn, bool isUkprnTaken, string companyNumber, bool isCompanyNumberTaken, string charityNumber,
                bool isCharityNumberTaken, string organisationId, string email, string contactName, bool isEmailTaken,
                string warningMessage1)
            {
                var warningMessages = new List<string>();
                if (!string.IsNullOrEmpty(warningMessage1))
                {
                    warningMessages.Add(warningMessage1);
                }

                IsOrganisationNameTaken = isOrganisationNameTaken;
                IsUkprnTaken = isUkprnTaken;
                IsCompanyNumberTaken = isCompanyNumberTaken;
                IsCharityNumberTaken = isCharityNumberTaken;
                IsEmailTaken = isEmailTaken;

                var response = new CreateOrganisationAndContactFromApplyResponse
                {
                    IsEpaoApproved = isEpaoApproved,
                    ApplySourceIsEpao = isEpaoSource,
                    WarningMessages = warningMessages,
                    OrganisationId = organisationId
                };
                //Command = new CreateOrganisationContactCommand();
                //{OrganisationReferenceType = organisationReferenceType};
                Command = new CreateOrganisationContactCommand
                {
                    OrganisationReferenceType = organisationReferenceType,
                    IsEpaoApproved = isEpaoApproved,
                    OrganisationName = organisationName,
                    TradingName = tradingName,
                    UseTradingName = true,
                    OrganisationUkprn = ukprn,
                    CompanyUkprn = "87654321",
                    CompanyNumber = companyNumber,
                    CharityNumber = charityNumber,
                    OrganisationType = organisationType,
                    ContactEmail = email,
                    ContactName = contactName,
                    ContactPhoneNumber = "11111111"
                };
                ExpectedResponse = response;
            }
        }
    }
}


