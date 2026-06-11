using System;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Factories
{
    public static class IlrFactory
    {
        public static IlrModel ForStandard(StandardModel standard, DateTime now)
        {
            return new IlrModel
            {
                Id = Guid.NewGuid(),
                Uln = 123456789,
                GivenNames = "Alice",
                FamilyName = "Bobdotter",
                Ukprn = 12345678,
                StdCode = standard.LarsCode,
                LearnStartDate = now.AddDays(-10),
                FundingModel = 36,
                Source = HandlerBase.GetAcademicYear(now),
                CreatedAt = now,
                CompletionStatus = 2,
                PlannedEndDate = now.AddMonths(12)
            };
        }

        public static IlrModel WithDateOfBirth(this IlrModel ilr, DateTime? dateOfBirth)
        {
            ilr.DateOfBirth = dateOfBirth;
            return ilr;
        }

        public static IlrModel WithLearnStartDate(this IlrModel ilr, DateTime? learnStartDate)
        {
            ilr.LearnStartDate = learnStartDate;
            return ilr;
        }

        public static IlrModel WithPlannedEndDate(this IlrModel ilr, DateTime? plannedEndDate)
        {
            ilr.PlannedEndDate = plannedEndDate;
            return ilr;
        }
    }
}
