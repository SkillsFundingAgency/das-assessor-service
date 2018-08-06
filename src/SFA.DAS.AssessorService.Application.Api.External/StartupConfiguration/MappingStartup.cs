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
                cfg.AddProfile<BatchCertificateRequestProfile>();
                cfg.AddProfile<BatchCertificateResponseProfile>();
            });

            Mapper.AssertConfigurationIsValid();
        }
    }
}
