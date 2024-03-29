﻿using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Standards;
using Swashbuckle.AspNetCore.Filters;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class GetOptionsForAllStandardResponseExample : IExamplesProvider<object[]>
    {
        public object[] GetExamples()
        {
            return new[] {
                new StandardOptions { StandardCode = 6, StandardReference = "ST0156", Version = "1.0", CourseOption = new[] { "Overhead lines", "Substation fitting", "Underground cables" } },
                new StandardOptions { StandardCode = 7, StandardReference = "ST0184", Version = "1.0", CourseOption = new[] { "Card services", "Corporate/Commercial", "Retail", "Wealth", } },
                new StandardOptions { StandardCode = 314, StandardReference = "ST0018", Version = "1.1", CourseOption = new[] { "Container based system", "Soil based system" } }
            };
        }
    }
}
