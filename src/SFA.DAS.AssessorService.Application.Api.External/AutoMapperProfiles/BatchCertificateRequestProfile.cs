using AutoMapper;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class BatchCertificateRequestProfile : Profile
    {
        public BatchCertificateRequestProfile()
        {
            CreateMap<Messages.BatchCertificateRequest, AssessorService.Api.Types.Models.Certificates.Batch.BatchCertificateRequest>()
                .ForMember(x => x.CertificateData, opt => opt.MapFrom(source => Mapper.Map<Models.Certificates.CertificateData, Domain.JsonData.CertificateData>(source.CertificateData)))
                .ForMember(x => x.Uln, opt => opt.MapFrom(source => source.CertificateData.Learner.Uln))
                .ForMember(x => x.StandardCode, opt => opt.MapFrom(source => source.CertificateData.LearningDetails.StandardCode))
                .ForMember(x => x.FamilyName, opt => opt.MapFrom(source => source.CertificateData.Learner.FamilyName))
                .ForMember(x => x.UkPrn, opt => opt.MapFrom(source => source.UkPrn))
                .ForMember(x => x.Username, opt => opt.MapFrom(source => source.Username))
                .ForAllOtherMembers(x => x.Ignore());
        }
    }
}
