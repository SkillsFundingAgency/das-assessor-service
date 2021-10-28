using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class GetBatchCertificateLogResponseProfile : Profile
    {

        public GetBatchCertificateLogResponseProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.ExternalApi.Certificates.GetBatchCertificateLogsResponse, GetCertificateLogsResponse>()
                .ForMember(dest => dest.CertificateLogs, opt => 
                    opt.MapFrom(source => source.CertificateLogs.Select(log => Mapper.Map<AssessorService.Api.Types.Models.ExternalApi.Certificates.BatchCertificateLog, CertificateLog>(log))))
                .ForMember(dest => dest.ValidationErrors, opt => opt.MapFrom(source => source.ValidationErrors))
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}
