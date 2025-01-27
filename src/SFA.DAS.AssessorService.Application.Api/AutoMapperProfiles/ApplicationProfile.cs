using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Entities;


namespace SFA.DAS.AssessorService.Application.Api.AutoMapperProfiles
{

    public class ApplicationListProfile : Profile
    {
        public ApplicationListProfile()
        {
            CreateMap<ApplicationListItem, ApplicationSummaryItem>()
            .ForMember(dest => dest.Versions, opt => opt.Ignore())
            .ForMember(dest => dest.WithdrawalType, opt => opt.Ignore());
        }
    }
}
