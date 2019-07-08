using SFA.DAS.AssessorService.Application.Api.External.Models.Request;
using SFA.DAS.AssessorService.Application.Api.External.Models.Request.Certificates;
using SFA.DAS.AssessorService.Application.Api.External.Models.Request.Epa;
using Swashbuckle.AspNetCore.Examples;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class UpdateEpaExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<UpdateEpaRequest>
            {
                new UpdateEpaRequest
                {
                    RequestId = "1",
                    EpaReference = "09876543",
                    Standard = new Standard { StandardCode = 1 },
                    Learner = new Learner { FamilyName = "Smith", Uln = 1234567890 },
                    EpaDetails = new EpaDetails { Epas = new List<EpaRecord> { new EpaRecord { EpaDate = DateTime.UtcNow, EpaOutcome = "pass" } } }
                },
                new UpdateEpaRequest
                {
                    RequestId = "2",
                    EpaReference = "99999999",
                    Standard = new Standard { StandardReference = "ST0099" },
                    Learner = new Learner { FamilyName = "Hamilton", Uln = 9999999999 },
                    EpaDetails = new EpaDetails { Epas = new List<EpaRecord> { new EpaRecord { EpaDate = DateTime.UtcNow, EpaOutcome = "pass" } } }
                },
                new UpdateEpaRequest
                {
                    RequestId = "3",
                    EpaReference = "55555555",
                    Standard = new Standard { StandardCode = 555, StandardReference = "ST0555" },
                    Learner = new Learner { FamilyName = "Unknown", Uln = 5555555555 },
                    EpaDetails = new EpaDetails { Epas = new List<EpaRecord> { new EpaRecord { EpaDate = DateTime.UtcNow, EpaOutcome = "pass" } } }
                }
            };
        }
    }
}
