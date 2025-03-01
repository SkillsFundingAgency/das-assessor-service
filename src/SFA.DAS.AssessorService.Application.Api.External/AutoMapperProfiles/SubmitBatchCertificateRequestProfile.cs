﻿using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.External.Models.Internal;
using SFA.DAS.AssessorService.AutoMapperExtensions;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class SubmitBatchCertificateRequestProfile : ExplicitMappingProfileBase
    {
        public SubmitBatchCertificateRequestProfile()
        {
            CreateMap<SubmitBatchCertificateRequest, AssessorService.Api.Types.Models.ExternalApi.Certificates.SubmitBatchCertificateRequest>()
            .ForMember(dest => dest.RequestId, opt => opt.MapFrom(source => source.RequestId))
            .ForMember(dest => dest.Uln, opt => opt.MapFrom(source => source.Uln))
            .ForMember(dest => dest.FamilyName, opt => opt.MapFrom(source => source.FamilyName))
            .ForMember(dest => dest.StandardCode, opt => opt.MapFrom(source => source.StandardCode ?? 0))
            .ForMember(dest => dest.StandardReference, opt => opt.MapFrom(source => source.StandardReference))
            .ForMember(dest => dest.CertificateReference, opt => opt.MapFrom(source => source.CertificateReference))            
            .ForMember(dest => dest.UkPrn, opt => opt.MapFrom(source => source.UkPrn));
        }
    }
}