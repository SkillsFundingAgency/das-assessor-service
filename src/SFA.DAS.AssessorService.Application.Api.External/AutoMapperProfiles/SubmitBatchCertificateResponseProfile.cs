using AutoMapper;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class SubmitBatchCertificateResponseProfile : Profile
    {
        public SubmitBatchCertificateResponseProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.Certificates.Batch.SubmitBatchCertificateResponse, Messages.SubmitBatchCertificateResponse>()
            .ForMember(x => x.RequestId, opt => opt.MapFrom(source => source.RequestId))
            .ForMember(x => x.Certificate, opt => opt.MapFrom(source => Mapper.Map<Domain.Entities.Certificate, Models.Certificates.Certificate>(source.Certificate)))
            .ForMember(x => x.ValidationErrors, opt => opt.MapFrom(source => source.ValidationErrors))
            .ForAllOtherMembers(x => x.Ignore());
        }
    }
}
