﻿using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.AutoMapperExtensions;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class GetBatchCertificateResponseProfile : ExplicitMappingProfileBase
    {
        public GetBatchCertificateResponseProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.ExternalApi.Certificates.GetBatchCertificateResponse, GetCertificateResponse>()
            .ForMember(dest => dest.Certificate, opt => opt.MapFrom(source => source.Certificate))
            .ForMember(dest => dest.ValidationErrors, opt => opt.MapFrom(source => source.ValidationErrors));
        }
    }
}
