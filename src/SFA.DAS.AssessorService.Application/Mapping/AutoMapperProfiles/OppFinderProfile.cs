using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Mapping.AutoMapperProfiles
{
    public class OppFinderProfile : Profile
    {
        public OppFinderProfile()
        {
            CreateMap<OppFinderApprovedStandard, OppFinderApprovedSearchResult>()
                .ForMember(dest => dest.StandardCode, opt => opt.MapFrom(source => source.StandardCode))
                .ForMember(dest => dest.StandardReference, opt => opt.MapFrom(source => source.StandardReference))
                .ForMember(dest => dest.StandardName, opt => opt.MapFrom(source => source.StandardName))
                .ForMember(dest => dest.Versions, opt => opt.MapFrom(source => source.Versions))
                .ForMember(dest => dest.ActiveApprentices, opt => opt.MapFrom(source => source.ActiveApprentices))
                .ForMember(dest => dest.RegisteredEPAOs, opt => opt.MapFrom(source => source.RegisteredEPAOs));

            CreateMap<OppFinderNonApprovedStandard, OppFinderSearchResult>()
                .ForMember(dest => dest.StandardCode, opt => opt.Ignore())
                .ForMember(dest => dest.StandardReference, opt => opt.MapFrom(source => source.StandardReference))
                .ForMember(dest => dest.StandardName, opt => opt.MapFrom(source => source.StandardName));
        }
    }
}
