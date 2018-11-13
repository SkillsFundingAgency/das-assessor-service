using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles;

namespace SFA.DAS.AssessorService.Application.Api.External.StartupConfiguration
{
    public static class MappingStartup
    {
        public static void AddMappings()
        {
            Mapper.Reset();

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<CertificateDataProfile>();
                cfg.AddProfile<CertificateStatusProfile>();
                cfg.AddProfile<CertificateProfile>();
                cfg.AddProfile<BatchCertificateRequestProfile>();
                cfg.AddProfile<BatchCertificateResponseProfile>();
                cfg.AddProfile<GetCertificateResponseProfile>();
                cfg.AddProfile<SubmitBatchCertificateRequestProfile>();
                cfg.AddProfile<SubmitBatchCertificateResponseProfile>();
                cfg.AddProfile<SearchResultToCertificateProfile>();
            });

            Mapper.AssertConfigurationIsValid();
        }
    }
}
