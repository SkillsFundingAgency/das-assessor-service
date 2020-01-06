using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.CharityCommission;
using SFA.DAS.AssessorService.ApplyTypes.CharityCommission;

namespace SFA.DAS.AssessorService.Web.AutoMapperProfiles
{
    public class CharityCommissionSummaryProfile : Profile
    {
        public CharityCommissionSummaryProfile()
        {
            CreateMap<Charity, CharityCommissionSummary>()
                .ForMember(dest => dest.CharityName, opt => opt.MapFrom(source => source.Name))
                .ForMember(dest => dest.CharityNumber, opt => opt.MapFrom(source => source.CharityNumber))
                .ForMember(dest => dest.IncorporatedOn, opt => opt.MapFrom(source => source.IncorporatedOn))
                .ForMember(dest => dest.Trustees, opt => opt.MapFrom(source => source.Trustees))
                .AfterMap<MapManualEntryRequiredAction>()
                .ForAllOtherMembers(opt => opt.Ignore());
        }

        public class MapManualEntryRequiredAction : IMappingAction<Charity, CharityCommissionSummary>
        {
            public void Process(Charity source, CharityCommissionSummary destination)
            {
                if (destination != null
                     && (destination.Trustees is null || destination.Trustees.Count == 0))
                {
                    destination.TrusteeManualEntryRequired = true;
                }
            }
        }
    }

    public class CharityTrusteeProfile : Profile
    {
        public CharityTrusteeProfile()
        {
            CreateMap<Trustee, TrusteeInformation>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(source => source.Name))
                .ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}
