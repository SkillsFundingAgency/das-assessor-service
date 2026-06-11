using System;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.TestHelper;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Factories
{
    public static class LearnerFactory
    {
        public static LearnerModel From(
            IlrModel ilr,
            ApprovalsExtractModel approvalsExtract,
            StandardModel standard)
        {
            return new LearnerModel
            {
                Uln = ilr.Uln,
                GivenNames = ilr.GivenNames,
                FamilyName = ilr.FamilyName,
                UkPrn = ilr.Ukprn,
                StdCode = ilr.StdCode,
                LearnStartDate = ilr.LearnStartDate,
                FundingModel = ilr.FundingModel,
                CompletionStatus = ilr.CompletionStatus,
                PlannedEndDate = ilr.PlannedEndDate,
                LatestIlrs = ilr.CreatedAt,

                ApprenticeshipId = approvalsExtract.ApprenticeshipId,
                Source = $"{ilr.Source}+App",  // when a learner is generated from an approvals record the '+App' is appended to the source
                Version = approvalsExtract.TrainingCourseVersion,
                VersionConfirmed = 1,
                StandardUId = approvalsExtract.StandardUId,
                LastUpdated = approvalsExtract.UpdatedOn?.Date,
                LatestApprovals = approvalsExtract.UpdatedOn,
                ApprovalsPaymentStatus = (short)approvalsExtract.PaymentStatus,
                EmployerAccountId = approvalsExtract.EmployerAccountId,
                EmployerName = approvalsExtract.EmployerName,

                StandardReference = standard.IFateReferenceNumber,
                StandardName = standard.Title,
                EstimatedEndDate = ilr.PlannedEndDate?.Date.GetEndOfMonth(),

                IsTransfer = 0
            };
        }

        public static LearnerModel From(
            IlrModel ilr,
            StandardModel standard)
        {
            return new LearnerModel
            {
                Id = null,

                // From ILR
                Uln = ilr.Uln,
                GivenNames = ilr.GivenNames,
                FamilyName = ilr.FamilyName,
                UkPrn = ilr.Ukprn,
                StdCode = ilr.StdCode,
                LearnStartDate = ilr.LearnStartDate,
                FundingModel = ilr.FundingModel,
                Source = ilr.Source,
                CompletionStatus = ilr.CompletionStatus,
                PlannedEndDate = ilr.PlannedEndDate,
                LatestIlrs = ilr.CreatedAt,
                DateOfBirth = ilr.DateOfBirth,

                // No approvals extract
                ApprenticeshipId = null,
                LearnRefNumber = null,
                ApprovalsStopDate = null,
                ApprovalsPauseDate = null,
                ApprovalsCompletionDate = null,
                ApprovalsPaymentStatus = null,
                LatestApprovals = null,
                EmployerAccountId = null,
                EmployerName = null,

                // From standard
                Version = standard.Version,
                VersionConfirmed = 1,
                CourseOption = null,
                StandardUId = standard.StandardUId,
                StandardReference = standard.IFateReferenceNumber,
                StandardName = standard.Title,

                // Derived
                LastUpdated = ilr.CreatedAt.Date,
                EstimatedEndDate = ilr.PlannedEndDate?.Date.GetEndOfMonth(),

                // Defaults / nulls
                EpaOrgId = null,
                DelLocPostCode = null,
                LearnActEndDate = null,
                WithdrawReason = null,
                Outcome = null,
                AchDate = null,
                OutGrade = null,
                IsTransfer = 0,
                DateTransferIdentified = null
            };
        }

        public static LearnerModel WithDateOfBirth(this LearnerModel learner, DateTime dateOfBirth)
        {
            learner.DateOfBirth = dateOfBirth;
            return learner;
        }

        public static LearnerModel WithVersion(this LearnerModel learner, string version)
        {
            learner.Version = version;
            return learner;
        }

        public static LearnerModel WithVersionConfirmed(this LearnerModel learner, int versionConfirmed)
        {
            learner.VersionConfirmed = versionConfirmed;
            return learner;
        }

        public static LearnerModel WithStandardUId(this LearnerModel learner, string standardUId)
        {
            learner.StandardUId = standardUId;
            return learner;
        }

        public static LearnerModel WithLastUpdated(this LearnerModel learner, DateTime? lastUpdated)
        {
            learner.LastUpdated = lastUpdated;
            return learner;
        }

        public static LearnerModel WithLatestApprovals(this LearnerModel learner, DateTime? latestApprovals)
        {
            learner.LatestApprovals = latestApprovals;
            return learner;
        }
    }
}
