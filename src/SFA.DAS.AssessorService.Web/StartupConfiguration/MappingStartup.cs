using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using SFA.DAS.AssessorService.Web.ViewModels.Search;

namespace SFA.DAS.AssessorService.Web.StartupConfiguration
{
    public static class MappingStartup
    {
        public static void AddMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ResultViewModel, SearchResult>();
                cfg.CreateMap<GetPrivateCertificateAlreadySubmittedResponse, SelectedStandardViewModel>();
                cfg.CreateMap<CertificateSummaryResponse, CertificateDetailApprovalViewModel>()
                    .ForMember(
                        dest => dest.ApprovedStatus, opt => opt.MapFrom(src => src.Status)
                    );
            });
        }
    }
}