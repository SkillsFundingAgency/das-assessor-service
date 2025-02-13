using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.AutoMapperExtensions;
using SFA.DAS.AssessorService.Web.ViewModels.Shared;

namespace SFA.DAS.AssessorService.Web.AutoMapperProfiles
{
    public class StandardVersionViewModelProfile : Profile
    {
        public StandardVersionViewModelProfile()
        {
            CreateMap<StandardVersionViewModel, StandardVersion>()
                    .ReverseMap();
        }
    }
}