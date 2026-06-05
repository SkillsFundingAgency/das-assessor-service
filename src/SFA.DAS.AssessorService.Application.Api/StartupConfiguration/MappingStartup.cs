using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Mapping.AutoMapperProfiles;

namespace SFA.DAS.AssessorService.Application.Api.StartupConfiguration
{
    public static class MappingStartup
    {
        public static void AddMappings(this IServiceCollection services, ILogger logger)
        {
            services.AddAutoMapper(cfg => { }, typeof(Startup).Assembly, typeof(ApplicationResponseProfile).Assembly);
        }
    }
}