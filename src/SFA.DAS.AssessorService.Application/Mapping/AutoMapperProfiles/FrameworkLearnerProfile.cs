using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;
using SFA.DAS.AssessorService.Application.Mapping.CustomResolvers;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Mapping.AutoMapperProfiles
{
    public class FrameworkLearnerProfile : Profile
    {
        public FrameworkLearnerProfile()
        {
            CreateMap<FrameworkLearner, FrameworkSearchResult>();

            CreateMap<FrameworkLearner, GetFrameworkCertificateResult>()
                .ForMember(dest => dest.CertificateNumber, opt => opt.MapFrom(src => src.ApprenticeReference))
                .ForMember(dest => dest.QualificationsAndAwardingBodies, opts => opts.MapFrom<QualificationDetailsResolver>());
        }
    }
}
