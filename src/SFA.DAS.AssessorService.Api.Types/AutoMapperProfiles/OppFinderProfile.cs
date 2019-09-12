using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.AutoMapperProfiles
{
    public class OppFinderProfile : Profile
    {
        public OppFinderProfile()
        {
            CreateMap<OppFinderApprovedStandard, OppFinderApprovedSearchResult>()
                .ForMember(dest => dest.StandardCode, opt => opt.MapFrom(source => source.StandardCode))
                .ForMember(dest => dest.StandardReference, opt => opt.MapFrom(source => source.StandardReference))
                .ForMember(dest => dest.StandardName, opt => opt.MapFrom(source => source.StandardName))
                .ForMember(dest => dest.ActiveApprentices, opt => opt.MapFrom(source => source.ActiveApprentices))
                .ForMember(dest => dest.RegisteredEPAOs, opt => opt.MapFrom(source => source.RegisteredEPAOs))
                .ForAllOtherMembers(dest => dest.Ignore());

            CreateMap<OppFinderApprovedStandard, OppFinderSearchResult>()
                .ForMember(dest => dest.StandardCode, opt => opt.MapFrom(source => source.StandardCode))
                .ForMember(dest => dest.StandardReference, opt => opt.MapFrom(source => source.StandardReference))
                .ForMember(dest => dest.StandardName, opt => opt.MapFrom(source => source.StandardName))
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}
