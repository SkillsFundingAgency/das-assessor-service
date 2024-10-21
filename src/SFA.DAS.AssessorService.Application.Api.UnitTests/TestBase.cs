using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.AutoMapperProfiles;
using SFA.DAS.AssessorService.Domain.Entities;
using System;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests
{
    public class TestBase
    {
        protected IMapper Mapper;

        public TestBase()
        {
            var services = new ServiceCollection();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var serviceProvider = services.BuildServiceProvider();
            Mapper = serviceProvider.GetRequiredService<IMapper>();
        }
    }
}
