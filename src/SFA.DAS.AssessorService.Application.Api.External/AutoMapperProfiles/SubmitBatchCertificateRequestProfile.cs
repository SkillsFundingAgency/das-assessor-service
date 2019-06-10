﻿using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.External.Models.Request;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class SubmitBatchCertificateRequestProfile : Profile
    {
        public SubmitBatchCertificateRequestProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.Certificates.Batch.SubmitBatchCertificateRequest, SubmitBatchCertificateRequest>()
            .ForMember(x => x.RequestId, opt => opt.MapFrom(source => source.RequestId))
            .ForMember(x => x.Uln, opt => opt.MapFrom(source => source.Uln))
            .ForMember(x => x.FamilyName, opt => opt.MapFrom(source => source.FamilyName))
            .ForMember(x => x.StandardCode, opt => opt.MapFrom(source => source.StandardCode))
            .ForMember(x => x.StandardReference, opt => opt.MapFrom(source => source.StandardReference))
            .ForMember(x => x.CertificateReference, opt => opt.MapFrom(source => source.CertificateReference))            
            .ForMember(x => x.UkPrn, opt => opt.MapFrom(source => source.UkPrn))
            .ForMember(x => x.Email, opt => opt.MapFrom(source => source.Email))
            .ForAllOtherMembers(x => x.Ignore());
        }
    }
}