﻿using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Mapping.AutoMapperProfiles
{
    public class FrameworkLearnerSearchResultProfile : Profile
    {
        public FrameworkLearnerSearchResultProfile()
        {
            CreateMap<FrameworkLearner, FrameworkLearnerSearchResponse>();
        }
    }
}
