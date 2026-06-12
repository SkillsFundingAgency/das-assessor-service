using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles;
using SFA.DAS.AssessorService.AutoMapperExtensions;
using System.Reflection;

namespace SFA.DAS.AssessorService.Application.Api.External.StartupConfiguration
{
    public static class MappingStartup
    {
        public static void AddMappings(this IServiceCollection services)
        {
            services.AddAutoMapper(
                typeof(CertificateDataProfile).Assembly
            );
        }
    }
}
