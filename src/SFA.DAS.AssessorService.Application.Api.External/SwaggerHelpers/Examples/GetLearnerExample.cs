using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Epa;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Learners;
using Swashbuckle.AspNetCore.Examples;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class GetLearnerExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new GetLearner
            {
                LearnerData = new LearnerData
                {
                    Standard = new Standard { StandardCode = 1, StandardReference = "ST0001", Level = 1, StandardName = "Example Standard" },
                    Learner = new Learner { GivenNames = "John", FamilyName = "Smith", Uln = 1234567890 },
                    LearningDetails = new LearningDetails { LearningStartDate = DateTime.UtcNow.AddYears(-1), ProviderUkPrn = 12345678, ProviderName = "Example Provider" },
                },
                Status = new Models.Response.Learners.Status { CurrentStatus = "1" },
                EpaDetails = new EpaDetails
                {
                    LatestEpaDate = DateTime.UtcNow,
                    LatestEpaOutcome = "pass",
                    Epas = new List<EpaRecord> { new EpaRecord { EpaDate = DateTime.UtcNow, EpaOutcome = "pass" } }
                }
            };
        }
    }
}
