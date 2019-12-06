using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Learner;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{

    public class ImportLearnerDetailHandlerTestsBase
    {
        protected Mock<IIlrRepository> IlrRepository;
        protected Mock<ICertificateRepository> CertificateRepository;

        protected ImportLearnerDetailHandler Sut;

        protected ImportLearnerDetailResponse Response;

        protected const long LearnerFourUln = 444444444444;
        protected const int LearnerFourStdCode = 40;

        protected static Ilr LearnerOne = new Ilr
        {
            Id = new Guid(),
            Source = "1920",
            Uln = 11111111111,
            StdCode = 10,
            GivenNames = "GivenLearnerOne",
            FamilyName = "FamilyLearnerOne",
            UkPrn = 1111,
            LearnStartDate = DateTime.Now.AddDays(-100),
            EpaOrgId = "EPA000111",
            FundingModel = 99,
            ApprenticeshipId = 11111111,
            EmployerAccountId = 111111111,
            CreatedAt = DateTime.Now.AddDays(-1000),
            UpdatedAt = null,
            LearnRefNumber = "1111111",
            CompletionStatus = 1111,
            EventId = 1111,
            PlannedEndDate = DateTime.Now.AddDays(100),
            DelLocPostCode = "1111ONE",
            LearnActEndDate = DateTime.Now.AddDays(-50),
            WithdrawReason = 1111,
            Outcome = 1,
            AchDate = DateTime.Now.AddDays(-50),
            OutGrade = "Pass"
        };

        protected static Ilr LearnerTwo = new Ilr
        {
            Id = new Guid(),
            Source = "1920",
            Uln = 22222222222,
            StdCode = 20,
            GivenNames = "GivenLearnerTwo",
            FamilyName = "FamilyLearnerTwo",
            UkPrn = 2222,
            LearnStartDate = DateTime.Now.AddDays(-200),
            EpaOrgId = "EPA000222",
            FundingModel = 36,
            ApprenticeshipId = 222222222,
            EmployerAccountId = 22222222,
            CreatedAt = DateTime.Now.AddDays(-1000),
            UpdatedAt = DateTime.Now.AddDays(-500),
            LearnRefNumber = "22222222",
            CompletionStatus = 2222,
            EventId = 2222,
            PlannedEndDate = DateTime.Now.AddDays(200),
            DelLocPostCode = "2222TWO",
            LearnActEndDate = DateTime.Now.AddDays(-20),
            WithdrawReason = 2222,
            Outcome = 2,
            AchDate = DateTime.Now.AddDays(-20),
            OutGrade = "Fail"
        };

        protected static Ilr LearnerThree = new Ilr
        {
            Id = new Guid(),
            Source = "2021",
            Uln = 333333333333,
            StdCode = 30,
            GivenNames = "GivenLearnerThree",
            FamilyName = "FamilyLearnerThree",
            UkPrn = 3333,
            LearnStartDate = DateTime.Now.AddDays(-200),
            EpaOrgId = "EPA000333",
            FundingModel = 36,
            ApprenticeshipId = 3333333333,
            EmployerAccountId = 33333333,
            CreatedAt = DateTime.Now.AddDays(-7000),
            UpdatedAt = DateTime.Now.AddDays(-300),
            LearnRefNumber = "33333333",
            CompletionStatus = 33333,
            EventId = 33333,
            PlannedEndDate = DateTime.Now.AddDays(300),
            DelLocPostCode = "3333THREE",
            LearnActEndDate = DateTime.Now.AddDays(-30),
            WithdrawReason = 33333,
            Outcome = 3,
            AchDate = DateTime.Now.AddDays(-30),
            OutGrade = "Pass"
        };

        protected static Ilr LearnerFive = new Ilr
        {
            Id = new Guid(),
            Source = "2021",
            Uln = 5555555555,
            StdCode = 30,
            GivenNames = "GivenLearnerFive",
            FamilyName = "FamilyLearnerFive",
            UkPrn = 55555,
            LearnStartDate = DateTime.Now.AddDays(-200),
            EpaOrgId = null,
            FundingModel = 36,
            ApprenticeshipId = 555555555,
            EmployerAccountId = 55555555,
            CreatedAt = DateTime.Now.AddDays(-7000),
            UpdatedAt = DateTime.Now.AddDays(-300),
            LearnRefNumber = "55555555",
            CompletionStatus = 55555,
            EventId = 55555,
            PlannedEndDate = DateTime.Now.AddDays(300),
            DelLocPostCode = "5555FIVE5555",
            LearnActEndDate = DateTime.Now.AddDays(-30),
            WithdrawReason = null,
            Outcome = null,
            AchDate = null,
            OutGrade = null
        };

        protected static Certificate LearnerOneCertificate = new Certificate
        {
            Uln = LearnerOne.Uln,
            StandardCode = LearnerOne.StdCode
        };

        protected static Certificate LearnerThreeCertificate = new Certificate
        {
            Uln = LearnerThree.Uln,
            StandardCode = LearnerThree.StdCode
        };

        protected ImportLearnerDetailRequest CreateImportLearnerDetailRequest(string source, long? ukprn, long? uln, int? stdCode,
            int? fundingModel, string givenNames, string familyName, DateTime? learnStartDate, DateTime? plannedEndDate,
            int? completionStatus, string learnRefNumber, string delLocPostCode)
        {
            return new ImportLearnerDetailRequest
            {
                Source = source,
                Ukprn = ukprn,
                Uln = uln,
                StdCode = stdCode,
                FundingModel = fundingModel,
                GivenNames = givenNames,
                FamilyName = familyName,
                LearnStartDate = learnStartDate,
                PlannedEndDate = plannedEndDate,
                CompletionStatus = completionStatus,
                LearnRefNumber = learnRefNumber,
                DelLocPostCode = delLocPostCode
            };
        }

        protected ImportLearnerDetailRequest CreateImportLearnerDetailRequest(string source, long? ukprn, long? uln, int? stdCode,
            int? fundingModel, string givenNames, string familyName, string epaOrgId, DateTime? learnStartDate, DateTime? plannedEndDate,
            int? completionStatus, string learnRefNumber, string delLocPostCode, DateTime? learnActEndDate, int? withdrawReason,
            int? outcome, DateTime? achDate, string outGrade)
        {
            return new ImportLearnerDetailRequest
            {
                Source = source,
                Ukprn = ukprn,
                Uln = uln,
                StdCode = stdCode,
                FundingModel = fundingModel,
                GivenNames = givenNames,
                FamilyName = familyName,
                EpaOrgId = epaOrgId,
                LearnStartDate = learnStartDate,
                PlannedEndDate = plannedEndDate,
                CompletionStatus = completionStatus,
                LearnRefNumber = learnRefNumber,
                DelLocPostCode = delLocPostCode,
                LearnActEndDate = learnActEndDate,
                WithdrawReason = withdrawReason,
                Outcome = outcome,
                AchDate = achDate,
                OutGrade = outGrade
            };
        }

        protected ImportLearnerDetailRequest CreateImportLearnerDetailRequest(Ilr ilr)
        {
            if(ilr != null)
            {
                return new ImportLearnerDetailRequest
                {
                    Source = ilr.Source,
                    Ukprn = ilr.UkPrn,
                    Uln = ilr.Uln,
                    StdCode = ilr.StdCode,
                    FundingModel = ilr.FundingModel,
                    GivenNames = ilr.GivenNames,
                    FamilyName = ilr.FamilyName,
                    LearnStartDate = ilr.LearnStartDate,
                    PlannedEndDate = ilr.PlannedEndDate,
                    CompletionStatus = ilr.CompletionStatus,
                    LearnRefNumber = ilr.LearnRefNumber,
                    DelLocPostCode = ilr.DelLocPostCode
                };
            }
            else
            {
                return new ImportLearnerDetailRequest
                {
                    Source = "2021",
                    Ukprn = 44444444,
                    Uln = 444444444444,
                    StdCode = 44,
                    FundingModel = 99,
                    GivenNames = "Four",
                    FamilyName = "Four",
                    LearnStartDate = DateTime.Now.AddDays(-50),
                    PlannedEndDate = DateTime.Now.AddDays(50),
                    CompletionStatus = 0,
                    LearnRefNumber = "44444",
                    DelLocPostCode = "44FOUR44"
                };
            }
        }

        protected ImportLearnerDetailRequest Request;

        protected void BaseArrange()
        {
            IlrRepository = new Mock<IIlrRepository>();
            IlrRepository.Setup(r => r.Get(LearnerOne.Uln, LearnerOne.StdCode)).ReturnsAsync(LearnerOne);
            IlrRepository.Setup(r => r.Get(LearnerTwo.Uln, LearnerTwo.StdCode)).ReturnsAsync(LearnerTwo);
            IlrRepository.Setup(r => r.Get(LearnerThree.Uln, LearnerThree.StdCode)).ReturnsAsync(LearnerThree);
            IlrRepository.Setup(r => r.Get(LearnerFive.Uln, LearnerFive.StdCode)).ReturnsAsync(LearnerFive);

            CertificateRepository = new Mock<ICertificateRepository>();
            CertificateRepository.Setup(r => r.GetCertificate(LearnerOne.Uln, LearnerOne.StdCode)).ReturnsAsync(LearnerOneCertificate);
            CertificateRepository.Setup(r => r.GetCertificate(LearnerTwo.Uln, LearnerTwo.StdCode)).ReturnsAsync((Certificate)null);
            CertificateRepository.Setup(r => r.GetCertificate(LearnerThree.Uln, LearnerThree.StdCode)).ReturnsAsync(LearnerThreeCertificate);

            Sut = new ImportLearnerDetailHandler(IlrRepository.Object, CertificateRepository.Object,
                new Mock<ILogger<ImportLearnerDetailHandler>>().Object);

        }
    }
}