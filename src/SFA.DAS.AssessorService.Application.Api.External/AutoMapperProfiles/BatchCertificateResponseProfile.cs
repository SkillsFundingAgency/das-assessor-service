using AutoMapper;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class BatchCertificateResponseProfile : Profile
    {
        public BatchCertificateResponseProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.Certificates.Batch.BatchCertificateResponse, Messages.BatchCertificateResponse>()
                    .ForMember(x => x.ProvidedCertificateData, opt => opt.MapFrom(source => Mapper.Map<Domain.JsonData.CertificateData, Models.Certificates.CertificateData>(source.ProvidedCertificateData)))
                    .ForPath(x => x.ProvidedCertificateData.CertificateReference, opt => opt.MapFrom(source => source.ProvidedCertificateReference))
                    .ForPath(x => x.ProvidedCertificateData.Learner.Uln, opt => opt.MapFrom(source => source.Uln))
                    .ForPath(x => x.ProvidedCertificateData.Learner.FamilyName, opt => opt.MapFrom(source => source.FamilyName))
                    .ForPath(x => x.ProvidedCertificateData.Standard.StandardCode, opt => opt.MapFrom(source => source.StandardCode))
                    .ForMember(x => x.Certificate, opt => opt.MapFrom(source => Mapper.Map<Domain.Entities.Certificate, Models.Certificates.Certificate>(source.Certificate)))
                    .ForMember(x => x.ValidationErrors, opt => opt.MapFrom(source => source.ValidationErrors))
                    .AfterMap<SetFamilyNameAction>()
                    .ForAllOtherMembers(x => x.Ignore());
        }

        public class SetFamilyNameAction : IMappingAction<AssessorService.Api.Types.Models.Certificates.Batch.BatchCertificateResponse, Messages.BatchCertificateResponse>
        {
            public void Process(AssessorService.Api.Types.Models.Certificates.Batch.BatchCertificateResponse source, Messages.BatchCertificateResponse destination)
            {
                if (destination.Certificate != null)
                {
                    destination.Certificate.CertificateData.Learner.FamilyName = source.FamilyName;
                }
            }
        }
    }
}
