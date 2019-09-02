using SFA.DAS.AssessorService.Application.Api.External.Models.Request;
using SFA.DAS.AssessorService.Application.Api.External.Models.Request.Certificates;
using SFA.DAS.AssessorService.Application.Api.External.Models.Request.Epa;
using SFA.DAS.AssessorService.Domain.Consts;
using Swashbuckle.AspNetCore.Examples;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class CreateEpaExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<CreateEpaRequest>
            {
                new CreateEpaRequest
                {
                    RequestId = "1",
                    Standard = new Standard { StandardCode = 1 },
                    Learner = new Learner { FamilyName = "Smith", Uln = 1234567890 },
                    EpaDetails = new EpaDetails { Epas = new List<EpaRecord> { new EpaRecord { EpaDate = DateTime.UtcNow, EpaOutcome = EpaOutcome.Pass } } }
                },
                new CreateEpaRequest
                {
                    RequestId = "2",
                    Standard = new Standard { StandardReference = "ST0099" },
                    Learner = new Learner { FamilyName = "Hamilton", Uln = 9999999999 },
                    EpaDetails = new EpaDetails { Epas = new List<EpaRecord> { new EpaRecord { EpaDate = DateTime.UtcNow, EpaOutcome = EpaOutcome.Fail } } }
                },
                new CreateEpaRequest
                {
                    RequestId = "3",
                    Standard = new Standard { StandardCode = 555, StandardReference = "ST0555" },
                    Learner = new Learner { FamilyName = "Unknown", Uln = 5555555555 },
                    EpaDetails = new EpaDetails { Epas = new List<EpaRecord> { new EpaRecord { EpaDate = DateTime.UtcNow, EpaOutcome = EpaOutcome.Pass } } }
                }
            };
        }
    }
}
