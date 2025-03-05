using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Mapping.AutoMapperProfiles
{
    public class LearnerSearchResultProfile : Profile
    {
        public LearnerSearchResultProfile()
        {
            CreateMap<Learner, LearnerSearchResponse>()
                .ForMember(dest => dest.Option, source => source.MapFrom(learner => learner.CourseOption))
                .ForMember(dest => dest.UpdatedAt, source => source.MapFrom(learner => learner.LastUpdated));
        }
    }

    public class LearnerStaffSearchProfile : Profile
    {
        public LearnerStaffSearchProfile()
        {
            CreateMap<Learner, StaffSearchItems>()
                .ForMember(q => q.StandardCode, opts => { opts.MapFrom(i => i.StdCode); });

        }
    }
}
