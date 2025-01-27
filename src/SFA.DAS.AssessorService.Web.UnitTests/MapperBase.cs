using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Web.AutoMapperProfiles;
using SFA.DAS.AssessorService.Web.Controllers.ManageUsers.ViewModels;
using SFA.DAS.AssessorService.Web.ViewModels.Organisation;
using SFA.DAS.AssessorService.Web.ViewModels.Search;
using SFA.DAS.AssessorService.Web.ViewModels.Shared;

namespace SFA.DAS.AssessorService.Application.Web.UnitTests
{
    public class MapperBase
    {
        public IMapper Mapper { get; private set; }

        public MapperBase()
        {
            var services = new ServiceCollection();

            services.AddAutoMapper(
                typeof(CharityCommissionSummaryProfile).Assembly
            );

            var serviceProvider = services.BuildServiceProvider();
            Mapper = serviceProvider.GetRequiredService<IMapper>();
        }
    }
}
