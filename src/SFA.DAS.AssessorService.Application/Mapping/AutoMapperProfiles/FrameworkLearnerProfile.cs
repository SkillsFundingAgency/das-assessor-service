﻿using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;
using SFA.DAS.AssessorService.Application.Mapping.CustomResolvers;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Mapping.AutoMapperProfiles
{
    public class FrameworkLearnerProfile : Profile
    {
        public FrameworkLearnerProfile()
        {
            CreateMap<FrameworkLearner, FrameworkLearnerSearchResponse>();

            CreateMap<FrameworkLearner, GetFrameworkLearnerResponse>()
                .ForMember(dest => dest.QualificationsAndAwardingBodies, opts => opts.MapFrom<QualificationDetailsResolver>());
        }
    }
}
