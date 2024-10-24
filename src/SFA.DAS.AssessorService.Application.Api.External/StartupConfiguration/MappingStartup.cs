using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles;

namespace SFA.DAS.AssessorService.Application.Api.External.StartupConfiguration
{
    public static class MappingStartup
    {
        public static void AddMappings(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<CertificateDataProfile>();
                cfg.AddProfile<CertificateProfile>();
                cfg.AddProfile<CertificateStatusProfile>();
                cfg.AddProfile<CreateBatchCertificateRequestProfile>();
                cfg.AddProfile<CreateBatchCertificateResponseProfile>();
                cfg.AddProfile<GetBatchCertificateResponseProfile>();
                cfg.AddProfile<SubmitBatchCertificateRequestProfile>();
                cfg.AddProfile<SubmitBatchCertificateResponseProfile>();
                cfg.AddProfile<UpdateBatchCertificateRequestProfile>();
                cfg.AddProfile<UpdateBatchCertificateResponseProfile>();

                cfg.AddProfile<GetLearnerProfile>();
                cfg.AddProfile<GetBatchLearnerResponseProfile>();

                cfg.AddProfile<EpaDetailsProfile>();
                cfg.AddProfile<EpaRecordProfile>();
                cfg.AddProfile<CreateBatchEpaRequestProfile>();
                cfg.AddProfile<CreateBatchEpaResponseProfile>();
                cfg.AddProfile<UpdateBatchEpaRequestProfile>();
                cfg.AddProfile<UpdateBatchEpaResponseProfile>();
            });
        }
    }
}
