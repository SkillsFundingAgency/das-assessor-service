using System;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using FluentAssertions.Primitives;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Interfaces.Validation;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Services;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Services
{
    [TestFixture]
    public class AnswerServiceinjectApplyOrganisationAndContactintoRegisterTests
    {
        //   Task<CreateOrganisationAndContactFromApplyResponse> InjectApplyOrganisationAndContactDetailsIntoRegister(CreateOrganisationContactCommand command);



        //public async Task<CreateOrganisationContactCommand> GatherAnswersForOrganisationAndContactForApplication(Guid applicationId)
        //{
        //    var tradingName = await GetAnswer(applicationId, "trading-name");
        //    var useTradingNameString = await GetAnswer(applicationId, "use-trading-name");
        //    var contactName = await GetAnswer(applicationId, "contact-name");
        //    var contactAddress1 = await GetAnswer(applicationId, "contact-address") ?? await GetAnswer(applicationId, "contact-address1");
        //    var contactAddress2 = await GetAnswer(applicationId, "contact-address2");
        //    var contactAddress3 = await GetAnswer(applicationId, "contact-address3");
        //    var contactAddress4 = await GetAnswer(applicationId, "contact-address4");
        //    var contactPostcode = await GetAnswer(applicationId, "contact-postcode");
        //    var contactEmail = await GetAnswer(applicationId, "contact-email");
        //    var contactPhoneNumber = await GetAnswer(applicationId, "contact-phone-number");
        //    var companyUkprn = await GetAnswer(applicationId, "company-ukprn");
        //    var companyNumber = await GetAnswer(applicationId, "company-number");
        //    var charityNumber = await GetAnswer(applicationId, "charity-number");
        //    var standardWebsite = await GetAnswer(applicationId, "standard-website");
        //    var organisation = await _applyApiClient.GetOrganisationForApplication(applicationId);
        //    var organisationName = organisation.Name;
        //    var organisationType = organisation.OrganisationType;
        //    var organisationUkprn = organisation.OrganisationUkprn;
        //    var organisationReferenceType = organisation?.OrganisationDetails?.OrganisationReferenceType;
        //    var isEpaoApproved = organisation.RoEPAOApproved;
        //    var useTradingName = useTradingNameString != null && (useTradingNameString.ToLower() == "yes" || useTradingNameString.ToLower() == "true" || useTradingNameString.ToLower() == "1");

        //    var command = new CreateOrganisationContactCommand
        //    (organisationName,
        //        organisationType,
        //        organisationUkprn?.ToString(),
        //        organisationReferenceType,
        //        isEpaoApproved,
        //        tradingName,
        //        useTradingName,
        //        contactName,
        //        contactAddress1,
        //        contactAddress2,
        //        contactAddress3,
        //        contactAddress4,
        //        contactPostcode,
        //        contactEmail,
        //        contactPhoneNumber,
        //        companyUkprn,
        //        companyNumber,
        //        charityNumber,
        //        standardWebsite);

        //    return command;
        //}

        private AnswerService _answerService;
        private Mock<IApplyApiClient> _mockApplyApiClient;
        private Mock<IRegisterQueryRepository> _mockRegisterQueryRepository;
        private Mock<IValidationService> _mockValidationService;
        private Mock<IAssessorValidationService> _mockAssessorValidationService;
        private Mock<IEpaOrganisationIdGenerator> _mockEpaOrganisationIdGenerator;
        private Mock<ILogger<AnswerService>> _mockLogger;
        private Mock<ISpecialCharacterCleanserService> _mockSpecialCharacterCleanserService;
        private Mock<IRegisterRepository> _mockRegisterRepository;

        private Guid _applicationId;


        [SetUp]
        public void Arrange()
        {
            //_applicationId = Guid.NewGuid();
            //_mockApplyApiClient = new Mock<IApplyApiClient>();
            //_mockRegisterQueryRepository = new Mock<IRegisterQueryRepository>();
            //_mockValidationService = new Mock<IValidationService>();
            //_mockAssessorValidationService = new Mock<IAssessorValidationService>();
            //_mockEpaOrganisationIdGenerator = new Mock<IEpaOrganisationIdGenerator>();
            //_mockLogger = new Mock<ILogger<AnswerService>>();
            //_mockSpecialCharacterCleanserService = new Mock<ISpecialCharacterCleanserService>();
            //_mockRegisterRepository = new Mock<IRegisterRepository>();
            ////_answerService = new AnswerService(
            ////    _mockApplyApiClient.Object,
            ////    Mock.Of<IValidationService>(),
            ////    Mock.Of<IAssessorValidationService>(),
            ////    Mock.Of<IRegisterQueryRepository>(),
            ////    Mock.Of<IRegisterRepository>(),
            ////    Mock.Of<IEpaOrganisationIdGenerator>(),
            ////    Mock.Of<ISpecialCharacterCleanserService>(),
            ////    Mock.Of<ILogger<AnswerService>>()
            ////);

            //_answerService = new AnswerService(
            //    _mockApplyApiClient.Object,
            //    _mockValidationService.Object,
            //    _mockAssessorValidationService.Object,
            //    _mockRegisterQueryRepository.Object,
            //    _mockRegisterRepository.Object,
            //    _mockEpaOrganisationIdGenerator.Object,
            //    _mockSpecialCharacterCleanserService.Object,
            //    _mockLogger.Object
            //);
        }


        //[TestCase("tag-one", "answer1")]
        //[TestCase("tag-two", null)]
        //[TestCase(null, null)]
        //public void WhenGettingAnswersForQuestionTag(string questionTag, string answer)
        //{
        //    _mockApplyApiClient.Setup(x => x.GetAnswer(_applicationId, questionTag))
        //        .Returns(Task.FromResult(new GetAnswersResponse { Answer = answer }));

        //    var expectedResult = _answerService.GetAnswer(_applicationId, questionTag).Result;

        //    Assert.AreEqual(expectedResult, answer);
        //}


    }
}


