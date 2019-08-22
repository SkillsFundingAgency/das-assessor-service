using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class UpdateBatchCertificateResponseProfile : Profile
    {
        public UpdateBatchCertificateResponseProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.ExternalApi.Certificates.BatchCertificateResponse, UpdateCertificateResponse>()
            .ForMember(dest => dest.RequestId, opt => opt.MapFrom(source => source.RequestId))
            .ForMember(dest => dest.Certificate, opt => opt.MapFrom(source => Mapper.Map<Domain.Entities.Certificate, Certificate>(source.Certificate)))
            .ForMember(dest => dest.ValidationErrors, opt => opt.MapFrom(source => source.ValidationErrors))
            .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}