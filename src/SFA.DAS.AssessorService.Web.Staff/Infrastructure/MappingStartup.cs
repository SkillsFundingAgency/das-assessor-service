using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;

namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    public static class MappingStartup
    {
        public static void AddMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<CertificateResponse, CertificateApprovalViewModel>();
            });
        }
    }
}