using System;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Factories
{
    public static class ApprovalsExtractFactory
    {
        public static ApprovalsExtractModel ForIlrAndStandard(IlrModel ilr, StandardModel standard, DateTime now)
        { 
            return new ApprovalsExtractModel
            {
                ApprenticeshipId = 12345,
                FirstName = ilr.GivenNames,
                LastName = ilr.FamilyName,
                ULN = ilr.Uln.ToString(),
                TrainingCode = standard.LarsCode,
                TrainingCourseVersion = standard.Version,
                TrainingCourseVersionConfirmed = false,
                StandardUId = standard.StandardUId,
                StartDate = now,
                CreatedOn = now,
                UpdatedOn = now,
                UKPRN = ilr.Ukprn,
                LearnRefNumber = "LEARN123",
                PaymentStatus = 1,
                EmployerAccountId = 12345,
                EmployerName = "Bob"
            };
        }
    }
}
