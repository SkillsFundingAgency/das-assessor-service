using AutoMapper;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class StandardOptionsProfile : Profile
    {
        public StandardOptionsProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.Standards.StandardCollation, Models.Standards.StandardOptions>()
                .ForMember(x => x.StandardCode, opt => opt.MapFrom(source => source.StandardId))
                .ForMember(x => x.StandardReference, opt => opt.MapFrom(source => source.ReferenceNumber))
                .ForMember(x => x.CourseOption, opt => opt.MapFrom(source => source.Options))
                .AfterMap<CollapseEmptySequenceAction>()
                .ForAllOtherMembers(x => x.Ignore());
        }

        public class CollapseEmptySequenceAction : IMappingAction<AssessorService.Api.Types.Models.Standards.StandardCollation, Models.Standards.StandardOptions>
        {
            public void Process(AssessorService.Api.Types.Models.Standards.StandardCollation source, Models.Standards.StandardOptions destination)
            {
                if (destination.CourseOption is null || !destination.CourseOption.Any())
                {
                    destination.CourseOption = null;
                }
            }
        }
    }
}
