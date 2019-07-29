using AutoMapper;
using System;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.AutoMapperProfiles
{
   
    public class AssessorServiceOrganisationTypeProfile : Profile
    {
        public AssessorServiceOrganisationTypeProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.AO.OrganisationType, OrganisationType>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(source => source.Type))
                .ForMember(dest => dest.TypeDescription, opt => opt.MapFrom(source => source.TypeDescription))
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}
