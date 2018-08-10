using AutoMapper;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class SubmitBatchCertificateRequestProfile : Profile
    {
        public SubmitBatchCertificateRequestProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.Certificates.Batch.SubmitBatchCertificateRequest, Messages.SubmitBatchCertificateRequest>()
            .ForMember(x => x.Uln, opt => opt.MapFrom(source => source.Uln))
            .ForMember(x => x.FamilyName, opt => opt.MapFrom(source => source.FamilyName))
            .ForMember(x => x.StandardCode, opt => opt.MapFrom(source => source.StandardCode))
            .ForMember(x => x.UkPrn, opt => opt.MapFrom(source => source.UkPrn))
            .ForMember(x => x.Username, opt => opt.MapFrom(source => source.Username))
            .ForAllOtherMembers(x => x.Ignore());
        }
    }
}