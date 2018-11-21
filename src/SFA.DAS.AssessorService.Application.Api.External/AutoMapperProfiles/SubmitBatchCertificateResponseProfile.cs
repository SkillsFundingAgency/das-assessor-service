using AutoMapper;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class SubmitBatchCertificateResponseProfile : Profile
    {
        public SubmitBatchCertificateResponseProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.Certificates.Batch.SubmitBatchCertificateResponse, Messages.SubmitBatchCertificateResponse>()
            .ForMember(x => x.Uln, opt => opt.MapFrom(source => source.Uln))
            .ForMember(x => x.FamilyName, opt => opt.MapFrom(source => source.FamilyName))
            .ForMember(x => x.StandardCode, opt => opt.MapFrom(source => source.StandardCode))
            .ForMember(x => x.CertificateReference, opt => opt.MapFrom(source => source.CertificateReference))
            .ForMember(x => x.Certificate, opt => opt.MapFrom(source => Mapper.Map<Domain.Entities.Certificate, Models.Certificates.Certificate>(source.Certificate)))
            .ForMember(x => x.ValidationErrors, opt => opt.MapFrom(source => source.ValidationErrors))
            .ForAllOtherMembers(x => x.Ignore());
        }
    }
}
