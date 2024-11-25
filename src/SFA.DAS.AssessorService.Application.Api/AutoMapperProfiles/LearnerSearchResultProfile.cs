using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.AutoMapperExtensions;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.Api.AutoMapperProfiles
{
    public class LearnerSearchResultProfile : Profile
    {
        public LearnerSearchResultProfile()
        {
            CreateMap<Learner, SearchResult>()
                .IgnoreUnmappedMembers()
                .ForMember(dest => dest.Option, source => source.MapFrom(learner => learner.CourseOption))
                .ForMember(dest => dest.UpdatedAt, source => source.MapFrom(learner => learner.LastUpdated)); 
        }
    }
}
