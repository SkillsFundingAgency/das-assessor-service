using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class CreateEpaResponseExample : IExamplesProvider<List<CreateEpaResponse>>
    {
        public List<CreateEpaResponse> GetExamples()
        {
            return new List<CreateEpaResponse>
            {
                new CreateEpaResponse
                {
                    RequestId = "1",
                    EpaReference = "09876543",
                    ValidationErrors = new List<string>()
                },
                new CreateEpaResponse
                {
                    RequestId = "2",
                    EpaReference = "99999999",
                    ValidationErrors = new List<string>()
                },
                new CreateEpaResponse
                {
                    RequestId = "3",
                    EpaReference = null,
                    ValidationErrors = new List<string>{ "ULN, FamilyName and Standard not found" }
                }
            };
        }
    }
}