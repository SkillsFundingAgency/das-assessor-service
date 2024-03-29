﻿using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Standards;
using Swashbuckle.AspNetCore.Filters;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class GetStandardVersionOptionsResponseExample : IExamplesProvider<StandardOptions>
    {
        public StandardOptions GetExamples()
        {
            return new StandardOptions { StandardCode = 6, StandardReference = "ST0156", Version = "1.0", CourseOption = new[] { "Overhead lines", "Substation fitting", "Underground cables" } };
        }
    }
}
