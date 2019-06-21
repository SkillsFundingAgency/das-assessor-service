using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class GetBatchCertificateResponseProfile : Profile
    {
        public GetBatchCertificateResponseProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.Certificates.Batch.GetBatchCertificateResponse, GetCertificateResponse>()
            .ForMember(dest => dest.Certificate, opt => opt.MapFrom(source => Mapper.Map<Domain.Entities.Certificate, Certificate>(source.Certificate)))
            .ForMember(dest => dest.ValidationErrors, opt => opt.MapFrom(source => source.ValidationErrors))
            .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}
