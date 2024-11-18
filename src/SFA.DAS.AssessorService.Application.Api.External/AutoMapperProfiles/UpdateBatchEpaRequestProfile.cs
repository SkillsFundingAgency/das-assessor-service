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
                .MapMatchingMembersAndIgnoreOthers()
                .ForMember(dest => dest.Uln, opt => opt.MapFrom(source => source.Learner.Uln))
                .ForMember(dest => dest.FamilyName, opt => opt.MapFrom(source => source.Learner.FamilyName))
                .ForMember(dest => dest.StandardCode, opt => opt.MapFrom(source => source.Standard.StandardCode ?? 0))
                .ForMember(dest => dest.StandardReference, opt => opt.MapFrom(source => source.Standard.StandardReference))
                .ForPath(dest => dest.EpaDetails.EpaReference, opt => opt.MapFrom(source => source.EpaReference))
                .ForMember(dest => dest.Version, opt => opt.MapFrom(source => source.LearningDetails.Version))
                .ForMember(dest => dest.CourseOption, opt => opt.MapFrom(source => source.LearningDetails.CourseOption));
        }
    }
}