using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.External.Models.Internal;
using SFA.DAS.AssessorService.Application.Api.External.Models.Request.Certificates;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class UpdateBatchCertificateRequestProfile : Profile
    {
        public UpdateBatchCertificateRequestProfile()
        {
            CreateMap<UpdateBatchCertificateRequest, AssessorService.Api.Types.Models.ExternalApi.Certificates.UpdateBatchCertificateRequest>()
                .MapMatchingMembersAndIgnoreOthers()
                .ForMember(dest => dest.RequestId, opt => opt.MapFrom(source => source.RequestId))
                .ForMember(dest => dest.Uln, opt => opt.MapFrom(source => source.CertificateData.Learner.Uln))
                .ForMember(dest => dest.FamilyName, opt => opt.MapFrom(source => source.CertificateData.Learner.FamilyName))
                .ForMember(dest => dest.StandardCode, opt => opt.MapFrom(source => source.CertificateData.Standard.StandardCode ?? 0))
                .ForMember(dest => dest.StandardReference, opt => opt.MapFrom(source => source.CertificateData.Standard.StandardReference))
                .ForMember(dest => dest.CertificateReference, opt => opt.MapFrom(source => source.CertificateData.CertificateReference))
                .ForMember(dest => dest.CertificateData, opt => opt.MapFrom(source => source.CertificateData))
                .ForMember(dest => dest.UkPrn, opt => opt.MapFrom(source => source.UkPrn));
        }
    }
}