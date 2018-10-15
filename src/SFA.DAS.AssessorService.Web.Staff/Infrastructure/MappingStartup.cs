using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
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
                cfg.CreateMap<CertificateResponse, CertificateDetailApprovalViewModel>()
                    .ForMember(
                        dest => dest.IsApproved, opt => opt.MapFrom(src => src.Status)
                    )
                    .ForMember(
                        dest => dest.FullName, opt => opt.MapFrom(src => src.CertificateData.FullName)
                    )
                    .ForMember(q => q.ApprovedRejected,
                        opts =>
                        {
                            opts.ResolveUsing<ApprovalsViewModelResolver>(); 
                            
                        });
            });
        }
    }
}