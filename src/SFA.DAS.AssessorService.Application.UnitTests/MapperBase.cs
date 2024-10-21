using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AssessorService.Application.Mapping.AutoMapperProfiles;

namespace SFA.DAS.AssessorService.Application.UnitTests
{
    public class MapperBase
    {
        public IMapper Mapper { get; private set; }

        public MapperBase()
        {
             var services = new ServiceCollection();
            services.AddAutoMapper(typeof(ApplicationResponseProfile).Assembly);

            var serviceProvider = services.BuildServiceProvider();
            Mapper = serviceProvider.GetRequiredService<IMapper>();

        }
    }
}
