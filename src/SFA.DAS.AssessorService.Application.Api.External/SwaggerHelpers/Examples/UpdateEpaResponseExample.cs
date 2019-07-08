using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using Swashbuckle.AspNetCore.Examples;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class UpdateEpaResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<UpdateEpaResponse>
            {
                new UpdateEpaResponse
                {
                    RequestId = "1",
                    EpaReference = "09876543",
                    ValidationErrors = new List<string>()
                },
                new UpdateEpaResponse
                {
                    RequestId = "2",
                    EpaReference = "99999999",
                    ValidationErrors = new List<string>()
                },
                new UpdateEpaResponse
                {
                    RequestId = "3",
                    EpaReference = null,
                    ValidationErrors = new List<string>{ "ULN, FamilyName and Standard not found" }
                }
            };
        }
    }
}
