using AutoMapper;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class BatchCertificateRequestProfile : Profile
    {
        public BatchCertificateRequestProfile()
        {
            CreateMap<Messages.BatchCertificateRequest, AssessorService.Api.Types.Models.Certificates.Batch.BatchCertificateRequest>()
                .Include<Messages.BatchCertificateRequest, AssessorService.Api.Types.Models.Certificates.Batch.CreateBatchCertificateRequest>()
                .Include<Messages.BatchCertificateRequest, AssessorService.Api.Types.Models.Certificates.Batch.UpdateBatchCertificateRequest>()

                .ForMember(x => x.RequestId, opt => opt.MapFrom(source => source.RequestId))
                .ForMember(x=> x.CertificateReference, opt => opt.MapFrom(source => source.CertificateData.CertificateReference))
                .ForMember(x => x.CertificateData, opt => opt.MapFrom(source => Mapper.Map<Models.Certificates.CertificateData, Domain.JsonData.CertificateData>(source.CertificateData)))
                .ForMember(x => x.Uln, opt => opt.MapFrom(source => source.CertificateData.Learner.Uln))
                .ForMember(x => x.StandardCode, opt => opt.MapFrom(source => source.CertificateData.Standard.StandardCode))
                .ForMember(x => x.StandardReference, opt => opt.MapFrom(source => source.CertificateData.Standard.StandardReference))
                .ForMember(x => x.FamilyName, opt => opt.MapFrom(source => source.CertificateData.Learner.FamilyName))
                .ForMember(x => x.UkPrn, opt => opt.MapFrom(source => source.UkPrn))
                .ForMember(x => x.Email, opt => opt.MapFrom(source => source.Email))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<Messages.BatchCertificateRequest, AssessorService.Api.Types.Models.Certificates.Batch.CreateBatchCertificateRequest>();
            CreateMap<Messages.BatchCertificateRequest, AssessorService.Api.Types.Models.Certificates.Batch.UpdateBatchCertificateRequest>();
        }
    }
}
