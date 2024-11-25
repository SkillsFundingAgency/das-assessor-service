using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;
using SFA.DAS.AssessorService.AutoMapperExtensions;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class CreateBatchCertificateResponseProfile : Profile
    {
        public CreateBatchCertificateResponseProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.ExternalApi.Certificates.BatchCertificateResponse, CreateCertificateResponse>()
            .ForMember(dest => dest.RequestId, opt => opt.MapFrom(source => source.RequestId))
            .ForMember(dest => dest.Certificate, opt => opt.MapFrom(source => source.Certificate))
            .ForMember(dest => dest.ValidationErrors, opt => opt.MapFrom(source => source.ValidationErrors))
            .IgnoreUnmappedMembers();
        }
    }
}