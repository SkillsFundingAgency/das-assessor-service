using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AssessorService.Web.AutoMapperProfiles;

namespace SFA.DAS.AssessorService.Web.StartupConfiguration
{
    public static class MappingStartup
    {
        public static void AddMappings(this IServiceCollection services)
        {
            services.AddAutoMapper(
                typeof(CharityCommissionSummaryProfile).Assembly
            );
        }
    }
}