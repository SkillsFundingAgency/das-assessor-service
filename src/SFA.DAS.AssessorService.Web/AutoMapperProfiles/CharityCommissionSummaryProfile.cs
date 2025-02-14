using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.CharityCommission;
using SFA.DAS.AssessorService.AutoMapperExtensions;

namespace SFA.DAS.AssessorService.Web.AutoMapperProfiles
{
    public class CharityCommissionSummaryProfile : ExplicitMappingProfileBase
    {
        public CharityCommissionSummaryProfile()
        {
            CreateMap<Charity, Domain.Entities.CharityCommissionSummary>()
                .ForMember(dest => dest.CharityName, opt => opt.MapFrom(source => source.Name))
                .ForMember(dest => dest.CharityNumber, opt => opt.MapFrom(source => source.CharityNumber))
                .ForMember(dest => dest.IncorporatedOn, opt => opt.MapFrom(source => source.IncorporatedOn))
                .ForMember(dest => dest.Trustees, opt => opt.MapFrom(source => source.Trustees))
                .AfterMap<MapManualEntryRequiredAction>();
        }

        public class MapManualEntryRequiredAction : IMappingAction<Charity, Domain.Entities.CharityCommissionSummary>
        {
            public void Process(Charity source, Domain.Entities.CharityCommissionSummary destination, ResolutionContext context)
            {
                if (destination != null
                     && (destination.Trustees is null || destination.Trustees.Count == 0))
                {
                    destination.TrusteeManualEntryRequired = true;
                }
            }
        }
    }

    public class CharityTrusteeProfile : ExplicitMappingProfileBase
    {
        public CharityTrusteeProfile()
        {
            CreateMap<Trustee, Domain.Entities.TrusteeInformation>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(source => source.Name));
        }
    }
}
