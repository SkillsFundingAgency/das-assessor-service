using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Interfaces.Validation;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Services;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Api.Types.Commands;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Services
{
    [TestFixture]
    public class AnswerServiceGatherAnswersTests
    {

        private AnswerService _answerService;
        private Mock<IApplyApiClient> _mockApplyApiClient;
        private Mock<IApiClient> _mockAssessorApiClient;

        private Guid _applicationId;


        [SetUp]
        public void Arrange()
        {
            _applicationId = Guid.NewGuid();
            _mockApplyApiClient = new Mock<IApplyApiClient>();
            _mockAssessorApiClient = new Mock<IApiClient>();
            _answerService = new AnswerService(
                _mockApplyApiClient.Object,
                _mockAssessorApiClient.Object
            );
        }


        [Test,TestCaseSource(nameof(CommandTestCases))]
        public void WhenGatheringAnswersForAnApplication(CommandTest commandTestSetup)
        {
            var expectedCommand = new CreateOrganisationContactCommand
            {
                UseTradingName = commandTestSetup.UseTradingName,
                TradingName = commandTestSetup.TradingName,
                OrganisationName = commandTestSetup.OrganisationName,
                IsEpaoApproved = commandTestSetup.IsEpaoApproved,
                OrganisationType = commandTestSetup.OrganisationType,
                OrganisationUkprn = commandTestSetup.OrganisationUkprn,
                OrganisationReferenceType = commandTestSetup.OrganisationReferenceType,
                ContactName = commandTestSetup.ContactName,
                ContactAddress1 = commandTestSetup.ContactAddress ?? commandTestSetup.ContactAddress1,
                ContactAddress2 = commandTestSetup.ContactAddress2,
                ContactAddress3 = commandTestSetup.ContactAddress3,
                ContactAddress4 = commandTestSetup.ContactAddress4,
                ContactPostcode = commandTestSetup.ContactPostcode,
                ContactEmail = commandTestSetup.ContactEmail,
                ContactPhoneNumber = commandTestSetup.ContactPhoneNumber,
                CompanyUkprn = commandTestSetup.CompanyUkprn,
                CompanyNumber = commandTestSetup.CompanyNumber,
                CharityNumber = commandTestSetup.CharityNumber,
                StandardWebsite = commandTestSetup.StandardWebsite,
                FinancialDueDate = commandTestSetup.FinancialDueDate,
                IsFinancialExempt = commandTestSetup.IsFinancialExempt,
            };

            int? organisationUkprn = null;

            if (int.TryParse(commandTestSetup.OrganisationUkprn, out int ukprnOut))
                organisationUkprn = ukprnOut;

            var organisationFromApplicationId = new SFA.DAS.AssessorService.ApplyTypes.Organisation
            {
                Name = commandTestSetup.OrganisationName,
                RoEPAOApproved = commandTestSetup.IsEpaoApproved != null && commandTestSetup.IsEpaoApproved.Value,
                OrganisationType = commandTestSetup.OrganisationType,
                OrganisationUkprn = organisationUkprn,
                OrganisationDetails = new OrganisationDetails
                {
                    OrganisationReferenceType = commandTestSetup.OrganisationReferenceType,
                    FHADetails = new ApplyTypes.FHADetails
                    {
                        FinancialDueDate = commandTestSetup.FinancialDueDate,
                        FinancialExempt = commandTestSetup.IsFinancialExempt
                    }
                }
            };

            _mockApplyApiClient.Setup(x => x.GetAnswer(_applicationId, It.IsAny<string>()))
                .Returns(Task.FromResult(new GetAnswersResponse { Answer = null }));
     
            foreach (var answerPair in commandTestSetup.AnswerPairs)
            {
                _mockApplyApiClient.Setup(x => x.GetAnswer(_applicationId, answerPair.QuestionTag))
                    .Returns(Task.FromResult(new GetAnswersResponse { Answer = answerPair.Answer }));
            }

            _mockApplyApiClient.Setup(x => x.GetOrganisationForApplication(_applicationId))
                .Returns(Task.FromResult(organisationFromApplicationId));

            var actualCommand = _answerService.GatherAnswersForOrganisationAndContactForApplication(_applicationId).Result;
   
            Assert.AreEqual(JsonConvert.SerializeObject(expectedCommand), JsonConvert.SerializeObject(actualCommand));
        }


        protected static IEnumerable<CommandTest> CommandTestCases
        {
            get
            {
                yield return new CommandTest("organisation name", "trading name 1", true, true, "true", "TrainingProvider","12343211", "RoEPAO", "Joe Contact", null, "address 1", "address 2", "address 3", "address 4", "CV1", "joe@cool.com", "43211234","11112222","RC333333","1221121","www.test.com", DateTime.MaxValue, false);
                yield return new CommandTest("organisation name", "trading name 1", true, true, "true", "TrainingProvider", "12343211", "RoEPAO", "Joe Contact", null, "address 1", "address 2", "address 3", "address 4", "CV1", "joe@cool.com", "43211234", "11112222", "RC333333", "1221121", "www.test.com", null, true);
                yield return new CommandTest("organisation name", "trading name 1", true, true, "yes", "TrainingProvider", "12343211", "RoEPAO", "Joe Contact", null, "address 1", "address 2", "address 3", "address 4", "CV1", "joe@cool.com", "43211234", "11112222", "RC333333", "1221121", "www.test.com", DateTime.MaxValue, false);
                yield return new CommandTest("organisation name", "trading name 1", true, true, "1", "TrainingProvider", "12343211", "RoEPAO", "Joe Contact", null, "address 1", "address 2", "address 3", "address 4", "CV1", "joe@cool.com", "43211234", "11112222", "RC333333", "1221121", "www.test.com", DateTime.MaxValue, false);
                yield return new CommandTest("organisation name", "trading name 1", true, false, "false", "TrainingProvider", "12343211", "RoEPAO", "Joe Contact", null, "address 1", "address 2", "address 3", "address 4", "CV1", "joe@cool.com", "43211234", "11112222", "RC333333", "1221121", "www.test.com", DateTime.MaxValue, false);
                yield return new CommandTest("organisation name", "trading name 1", true, false, "0", "TrainingProvider", "12343211", "RoEPAO", "Joe Contact", "address line 1", "address 1", "address 2", "address 3", "address 4", "CV1", "joe@cool.com", "43211234", "11112222", "RC333333", "1221121", "www.test.com", DateTime.MaxValue, false);
            }
        }


        public class CommandTest
        {
            public string OrganisationName { get; set; }
            public string OrganisationType { get; set; }
            public string OrganisationUkprn { get; set; }
            public bool? IsEpaoApproved { get; set; }
            public string TradingName { get; set; }
            public bool UseTradingName { get; set; }
            public string UseTradingNameString { get; set; }
            public string OrganisationReferenceType { get; set; }
            public string ContactName { get; set; }
            public string ContactAddress { get; set; }
            public string ContactAddress1 { get; set; }
            public string ContactAddress2 { get; set; }
            public string ContactAddress3 { get; set; }
            public string ContactAddress4 { get; set; }

            public string ContactPostcode { get; set; }
            public string ContactEmail { get; set; }
            public string ContactPhoneNumber { get; set; }
            public string CompanyUkprn { get; set; }
            public string CompanyNumber { get; set; }
            public string CharityNumber { get; set; }
            public string StandardWebsite { get; set; }

            public DateTime? FinancialDueDate { get; set; }
            public bool? IsFinancialExempt { get; set; }

            public List<GetAnswerPair> AnswerPairs { get; set; }
            public CommandTest(string organisationName, string tradingName, bool isEpaoApproved, bool useTradingName, string useTradingNameString, string organisationType, string organisationUkprn 
               , string organisationReferenceType, string contactName, string contactAddress, string contactAddress1, string contactAddress2, string contactAddress3, string contactAddress4, string contactPostcode
               , string contactEmail, string contactPhoneNumber, string companyUkprn, string companyNumber, string charityNumber, string standardWebsite, DateTime? financialDueDate, bool? isFinancialExempt)
            {
                OrganisationName = organisationName;
                OrganisationType = organisationType;
                OrganisationUkprn = organisationUkprn;
                IsEpaoApproved = isEpaoApproved;
                TradingName = tradingName;
                UseTradingName = useTradingName;
                UseTradingNameString = useTradingNameString;
                OrganisationReferenceType = organisationReferenceType;
                ContactName = contactName;
                ContactAddress = contactAddress;
                ContactAddress1 = contactAddress1;
                ContactAddress2 = contactAddress2;
                ContactAddress3 = contactAddress3;
                ContactAddress4 = contactAddress4;
                ContactPostcode = contactPostcode;
                ContactEmail = contactEmail;
                ContactPhoneNumber = contactPhoneNumber;
                CompanyUkprn = companyUkprn;
                CompanyNumber = companyNumber;
                CharityNumber = charityNumber;
                StandardWebsite = standardWebsite;
                FinancialDueDate = financialDueDate;
                IsFinancialExempt = isFinancialExempt;
                

                AnswerPairs = new List<GetAnswerPair>
                {
                    new GetAnswerPair("trading-name", tradingName),
                    new GetAnswerPair("use-trading-name", useTradingNameString),
                    new GetAnswerPair("contact-name", contactName),
                    new GetAnswerPair("contact-address1",contactAddress1),
                    new GetAnswerPair("contact-address2",contactAddress2),
                    new GetAnswerPair("contact-address3",contactAddress3),
                    new GetAnswerPair("contact-address4",contactAddress4),
                    new GetAnswerPair("contact-postcode",contactPostcode),
                    new GetAnswerPair("contact-email",contactEmail),
                    new GetAnswerPair("contact-phone-number",contactPhoneNumber),
                    new GetAnswerPair("company-ukprn",companyUkprn),
                    new GetAnswerPair("company-number",companyNumber),
                    new GetAnswerPair("charity-number",charityNumber),
                    new GetAnswerPair("standard-website",standardWebsite),
                    new GetAnswerPair("contact-address",contactAddress)
                };
            }
        }

        public class GetAnswerPair
        {
            public string QuestionTag { get; set; }
            public string Answer { get; set; }

            public GetAnswerPair(string questionTag, string answer)
            {
                QuestionTag = questionTag;
                Answer = answer;
            }
        }


        [Test, TestCaseSource(nameof(StandardCommandTestCases))]
        public void WhenGatheringAnswersForAnOrganisationStandard(StandardCommandTest commandTestSetup)
        {
            var expectedCommand = new CreateOrganisationStandardCommand
            { 
                CreatedBy = commandTestSetup.CreatedBy,
                OrganisationId = commandTestSetup.EndPointAssessorOrganisationId,
                StandardCode = commandTestSetup.StandardCode,
                EffectiveFrom = commandTestSetup.EffectiveFrom,
                DeliveryAreas = commandTestSetup.DeliveryAreas
            };

            var organisationFromApplicationId = new ApplyTypes.Organisation
            {
                Name = commandTestSetup.OrganisationName
            };

            var applicationFromApplicationId = new ApplyTypes.Application
            {
                Id = _applicationId,
                CreatedBy = commandTestSetup.CreatedBy,
                ApplicationData = new ApplicationData
                {
                    StandardCode = commandTestSetup.StandardCode
                }
            };

            _mockApplyApiClient.Setup(x => x.GetAnswer(_applicationId, It.IsAny<string>()))
                .Returns(Task.FromResult(new GetAnswersResponse { Answer = null }));

            foreach (var answerPair in commandTestSetup.AnswerPairs)
            {
                _mockApplyApiClient.Setup(x => x.GetAnswer(_applicationId, answerPair.QuestionTag))
                    .Returns(Task.FromResult(new GetAnswersResponse { Answer = answerPair.Answer }));
            }

            _mockApplyApiClient.Setup(x => x.GetApplication(_applicationId))
                .Returns(Task.FromResult(applicationFromApplicationId));

            _mockApplyApiClient.Setup(x => x.GetOrganisationForApplication(_applicationId))
                .Returns(Task.FromResult(organisationFromApplicationId));

            var assesorOrganisation = new AssessmentOrganisationSummary
            {
                Name = commandTestSetup.OrganisationName,
                Id = commandTestSetup.EndPointAssessorOrganisationId
            };

            _mockAssessorApiClient.Setup(x => x.SearchOrganisations(commandTestSetup.OrganisationName))
                .ReturnsAsync(new List<AssessmentOrganisationSummary> { assesorOrganisation });

            var actualCommand = _answerService.GatherAnswersForOrganisationStandardForApplication(_applicationId).Result;

            Assert.AreEqual(JsonConvert.SerializeObject(expectedCommand), JsonConvert.SerializeObject(actualCommand));
        }

        public class StandardCommandTest
        {
            public string OrganisationName { get; set; }
            public string CreatedBy { get; set; }
            public string EndPointAssessorOrganisationId { get; set; }
            public int StandardCode { get; set; }
            public DateTime EffectiveFrom { get; set; }
            public string DeliveryAreasString { get; set; }
            public List<string> DeliveryAreas => DeliveryAreasString?.Split(",").ToList();

            public List<GetAnswerPair> AnswerPairs { get; set; }
            public StandardCommandTest(string organisationName, string createdBy
               , string endPointAssessorOrganisationId, int standardCode, DateTime effectiveFrom, string deliveryAreasString)
            {
                OrganisationName = organisationName;
                CreatedBy = createdBy;
                EndPointAssessorOrganisationId = endPointAssessorOrganisationId;
                StandardCode = standardCode;
                EffectiveFrom = DateTime.Parse(effectiveFrom.ToString());
                DeliveryAreasString = deliveryAreasString;

                AnswerPairs = new List<GetAnswerPair>
                {
                    new GetAnswerPair("effective-from", effectiveFrom.ToString()),
                    new GetAnswerPair("delivery-areas", deliveryAreasString)
                };
            }
        }

        protected static IEnumerable<StandardCommandTest> StandardCommandTestCases
        {
            get
            {
                yield return new StandardCommandTest("organisation name", Guid.NewGuid().ToString(), "EPA0001", 1, DateTime.UtcNow.Date, "East Midlands");
            }
        }
    }
    
}
