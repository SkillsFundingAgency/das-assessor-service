using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.External.Models.Internal;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class SubmitBatchCertificateRequestProfile : Profile
    {
        public SubmitBatchCertificateRequestProfile()
        {
            CreateMap<SubmitBatchCertificateRequest, AssessorService.Api.Types.Models.ExternalApi.Certificates.SubmitBatchCertificateRequest>()
            .ForMember(dest => dest.RequestId, opt => opt.MapFrom(source => source.RequestId))
            .ForMember(dest => dest.Uln, opt => opt.MapFrom(source => source.Uln))
            .ForMember(dest => dest.FamilyName, opt => opt.MapFrom(source => source.FamilyName))
            .ForMember(dest => dest.StandardCode, opt => opt.MapFrom(source => source.StandardCode))
            .ForMember(dest => dest.StandardReference, opt => opt.MapFrom(source => source.StandardReference))
            .ForMember(dest => dest.CertificateReference, opt => opt.MapFrom(source => source.CertificateReference))            
            .ForMember(dest => dest.UkPrn, opt => opt.MapFrom(source => source.UkPrn))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(source => source.Email))
            .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}