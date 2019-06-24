using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Standards;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class StandardOptionsProfile : Profile
    {
        public StandardOptionsProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.Standards.StandardCollation, StandardOptions>()
                .ForMember(dest => dest.StandardCode, opt => opt.MapFrom(source => source.StandardId))
                .ForMember(dest => dest.StandardReference, opt => opt.MapFrom(source => source.ReferenceNumber))
                .ForMember(dest => dest.CourseOption, opt => opt.MapFrom(source => source.Options))
                .AfterMap<CollapseEmptySequenceAction>()
                .ForAllOtherMembers(dest => dest.Ignore());
        }

        public class CollapseEmptySequenceAction : IMappingAction<AssessorService.Api.Types.Models.Standards.StandardCollation, StandardOptions>
        {
            public void Process(AssessorService.Api.Types.Models.Standards.StandardCollation source, StandardOptions destination)
            {
                if (destination.CourseOption is null || !destination.CourseOption.Any())
                {
                    destination.CourseOption = null;
                }
            }
        }
    }
}
