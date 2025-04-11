using AutoMapper;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Mapping.AutoMapperProfiles
{
    public class ApplicationSummaryItemProfile : Profile
    {
        public ApplicationSummaryItemProfile()
        {
            CreateMap<ApplicationListItem, ApplicationSummaryItem>()
                .ForMember(dest => dest.Versions, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<string>>(src.Versions)));
        }

    }
}
