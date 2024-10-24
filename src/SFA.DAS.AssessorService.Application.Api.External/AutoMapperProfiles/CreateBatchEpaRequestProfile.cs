using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.External.Models.Internal;
using SFA.DAS.AssessorService.Application.Api.External.Models.Request.Epa;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class CreateBatchEpaRequestProfile : Profile
    {
        public CreateBatchEpaRequestProfile()
        {
            CreateMap<CreateBatchEpaRequest, AssessorService.Api.Types.Models.ExternalApi.Epas.CreateBatchEpaRequest>()
                .IgnoreAll()
                .ForMember(dest => dest.RequestId, opt => opt.MapFrom(source => source.RequestId))
                .ForMember(dest => dest.Uln, opt => opt.MapFrom(source => source.Learner.Uln))
                .ForMember(dest => dest.FamilyName, opt => opt.MapFrom(source => source.Learner.FamilyName))
                .ForMember(dest => dest.StandardCode, opt => opt.MapFrom(source => source.Standard.StandardCode ?? 0))
                .ForMember(dest => dest.StandardReference, opt => opt.MapFrom(source => source.Standard.StandardReference))
                .ForMember(dest => dest.EpaDetails, opt => opt.MapFrom(source => source.EpaDetails))
                .ForMember(dest => dest.UkPrn, opt => opt.MapFrom(source => source.UkPrn))
                .ForMember(dest => dest.Version, opt => opt.MapFrom(source => source.LearningDetails.Version))
                .ForMember(dest => dest.CourseOption, opt => opt.MapFrom(source => source.LearningDetails.CourseOption));
        }
    }
}