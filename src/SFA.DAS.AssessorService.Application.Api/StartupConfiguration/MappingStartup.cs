using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Mapping.AutoMapperProfiles;
using SFA.DAS.AssessorService.AutoMapperExtensions;
using SFA.DAS.AssessorService.Domain.Entities;
using System;

namespace SFA.DAS.AssessorService.Application.Api.StartupConfiguration
{
    public static class MappingStartup
    {
        public static void AddMappings(this IServiceCollection services, ILogger logger)
        {
            services.AddAutoMapper(
                typeof(Startup).Assembly,
                typeof(ApplicationResponseProfile).Assembly
            );
        }
    }
}