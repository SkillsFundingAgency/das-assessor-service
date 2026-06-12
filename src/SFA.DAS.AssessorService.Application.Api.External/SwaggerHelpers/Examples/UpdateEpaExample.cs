using SFA.DAS.AssessorService.Application.Api.External.Models.Request;
using SFA.DAS.AssessorService.Application.Api.External.Models.Request.Certificates;
using SFA.DAS.AssessorService.Application.Api.External.Models.Request.Epa;
using SFA.DAS.AssessorService.Domain.Consts;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class UpdateEpaExample : IExamplesProvider<List<UpdateEpaRequest>>
    {
        public List<UpdateEpaRequest> GetExamples()
        {
            return new List<UpdateEpaRequest>
            {
                new UpdateEpaRequest
                {
                    RequestId = "1",
                    EpaReference = "09876543",
                    Standard = new Standard { StandardCode = 1 },
                    Learner = new Learner { FamilyName = "Smith", Uln = 1234567890 },
                    LearningDetails = new LearningDetails{ CourseOption = "French", Version="1.0" },
                    EpaDetails = new EpaDetails { Epas = new List<EpaRecord> { new EpaRecord { EpaDate = DateTime.UtcNow, EpaOutcome = EpaOutcome.Pass } } }
                },
                new UpdateEpaRequest
                {
                    RequestId = "2",
                    EpaReference = "99999999",
                    Standard = new Standard { StandardReference = "ST0099" },
                    Learner = new Learner { FamilyName = "Hamilton", Uln = 9999999999 },
                    LearningDetails = new LearningDetails{ CourseOption = null, Version="1.0" },
                    EpaDetails = new EpaDetails { Epas = new List<EpaRecord> { new EpaRecord { EpaDate = DateTime.UtcNow, EpaOutcome = EpaOutcome.Pass } } }
                },
                new UpdateEpaRequest
                {
                    RequestId = "3",
                    EpaReference = "55555555",
                    Standard = new Standard { StandardCode = 555, StandardReference = "ST0555" },
                    Learner = new Learner { FamilyName = "Unknown", Uln = 5555555555 },
                    LearningDetails = new LearningDetails{ CourseOption = null, Version="1.0" },
                    EpaDetails = new EpaDetails { Epas = new List<EpaRecord> { new EpaRecord { EpaDate = DateTime.UtcNow, EpaOutcome = EpaOutcome.Pass } } }
                }
            };
        }
    }
}
