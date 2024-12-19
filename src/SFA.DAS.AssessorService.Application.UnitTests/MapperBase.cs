using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AssessorService.Application.Mapping.AutoMapperProfiles;
using SFA.DAS.AssessorService.AutoMapperExtensions;
using System.Reflection;

namespace SFA.DAS.AssessorService.Application.UnitTests
{
    public class MapperBase
    {
        public IMapper Mapper { get; private set; }

        public MapperBase()
        {
            var services = new ServiceCollection();
            services.AddAutoMapper(
                typeof(ApplicationResponseProfile).Assembly
            );

            var serviceProvider = services.BuildServiceProvider();
            Mapper = serviceProvider.GetRequiredService<IMapper>();

            string referencingAssemblyPath = Assembly.GetExecutingAssembly().Location;
            AutoMapAnalyser.FindAndPrintMapCalls(referencingAssemblyPath, Mapper.ConfigurationProvider);
        }
    }
}
