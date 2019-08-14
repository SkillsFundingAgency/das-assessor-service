using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.External.Models.Internal;
using SFA.DAS.AssessorService.Application.Api.External.Models.Request.Epa;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class UpdateBatchEpaRequestProfile : Profile
    {
        public UpdateBatchEpaRequestProfile()
        {
            CreateMap<UpdateBatchEpaRequest, AssessorService.Api.Types.Models.ExternalApi.Epas.UpdateBatchEpaRequest>()
                .ForMember(dest => dest.RequestId, opt => opt.MapFrom(source => source.RequestId))
                .ForMember(dest => dest.Uln, opt => opt.MapFrom(source => source.Learner.Uln))
                .ForMember(dest => dest.FamilyName, opt => opt.MapFrom(source => source.Learner.FamilyName))
                .ForMember(dest => dest.StandardCode, opt => opt.MapFrom(source => source.Standard.StandardCode ?? 0))
                .ForMember(dest => dest.StandardReference, opt => opt.MapFrom(source => source.Standard.StandardReference))
                .ForMember(dest => dest.EpaDetails, opt => opt.MapFrom(source => Mapper.Map<EpaDetails, Domain.JsonData.EpaDetails>(source.EpaDetails)))
                .ForPath(dest => dest.EpaDetails.EpaReference, opt => opt.MapFrom(source => source.EpaReference))
                .ForMember(dest => dest.UkPrn, opt => opt.MapFrom(source => source.UkPrn))
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}