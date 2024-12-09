using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Mapping.CustomResolvers;
using SFA.DAS.AssessorService.AutoMapperExtensions;
using SFA.DAS.AssessorService.Domain.DTOs;

namespace SFA.DAS.AssessorService.Application.Mapping.AutoMapperProfiles
{
    public class ApplicationResponseProfile : Profile
    {
        public ApplicationResponseProfile()
        {
            // Request going to Int API
            CreateMap<ApplySummary, ApplicationResponse>()
                .IgnoreUnmappedMembers()
                .ForMember(dest => dest.ContactName, opt => opt.MapFrom(source => source.CreatedByName))
                .ForMember(dest => dest.ContactEmail, opt => opt.MapFrom(source => source.CreatedByEmail))
                .ForMember(dest => dest.ApplicationType, opts => opts.MapFrom<ApplicationTypeResolver>());
        }
    }
}
