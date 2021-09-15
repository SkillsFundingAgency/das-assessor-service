using AutoMapper;

namespace SFA.DAS.AssessorService.Application.Mapping.AutoMapperProfiles
{
    public class ImportApprovalsProfile : Profile
    {
        public ImportApprovalsProfile()
        {
            CreateMap<Infrastructure.OuterApi.Learner, Domain.Entities.ApprovalsExtract>()
                //.ForMember(dst => dst.ULN, opt => opt.MapFrom(src => src.ULN))
            ;
        }
    }
}
