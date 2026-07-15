using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles;

namespace SFA.DAS.AssessorService.Application.Api.External.StartupConfiguration
{
    public static class MappingStartup
    {
        public static void AddMappings(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => { }, typeof(CertificateDataProfile).Assembly);
        }
    }
}
