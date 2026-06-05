using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AssessorService.Application.Mapping.AutoMapperProfiles;
using SFA.DAS.AssessorService.Application.Api.AutoMapperProfiles;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests
{
    public class TestBase
    {
        protected IMapper Mapper;

        public TestBase()
        {
            var services = new ServiceCollection();

            // pass assemblies as params
            services.AddAutoMapper(cfg => { }, typeof(TestBase).Assembly, typeof(ApplicationResponseProfile).Assembly);

            var serviceProvider = services.BuildServiceProvider();
            Mapper = serviceProvider.GetRequiredService<IMapper>();
        }
    }
}
