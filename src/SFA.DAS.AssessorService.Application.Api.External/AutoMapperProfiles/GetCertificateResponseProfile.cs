using AutoMapper;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class GetCertificateResponseProfile : Profile
    {
        public GetCertificateResponseProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.Certificates.Batch.GetBatchCertificateResponse, Messages.GetCertificateResponse>()
            .ForMember(x => x.Uln, opt => opt.MapFrom(source => source.Uln))
            .ForMember(x => x.FamilyName, opt => opt.MapFrom(source => source.FamilyName))
            .ForMember(x => x.StandardCode, opt => opt.MapFrom(source => source.StandardCode))
            .ForMember(x => x.Certificate, opt => opt.MapFrom(source => Mapper.Map<Domain.Entities.Certificate, Models.Certificates.Certificate>(source.Certificate)))
            .ForMember(x => x.ValidationErrors, opt => opt.MapFrom(source => source.ValidationErrors))
            .AfterMap<SetFamilyNameAction>()
            .ForAllOtherMembers(x => x.Ignore());
        }

        public class SetFamilyNameAction : IMappingAction<AssessorService.Api.Types.Models.Certificates.Batch.GetBatchCertificateResponse, Messages.GetCertificateResponse>
        {
            public void Process(AssessorService.Api.Types.Models.Certificates.Batch.GetBatchCertificateResponse source, Messages.GetCertificateResponse destination)
            {
                if (destination.Certificate != null)
                {
                    destination.Certificate.CertificateData.Learner.FamilyName = source.FamilyName;
                }
            }
        }
    }
}
