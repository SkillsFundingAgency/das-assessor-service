using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.Web.Staff.Automapper.CustomResolvers;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;

namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    public static class MappingStartup
    {
        public static void AddMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<CertificateSummaryResponse, CertificateDetailApprovalViewModel>()
                    .ForMember(
                        dest => dest.IsApproved, opt => opt.MapFrom(src => src.Status)
                    );
            });
        }
    }
}