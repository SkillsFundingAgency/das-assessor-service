using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Web.Controllers.ManageUsers.ViewModels;
using SFA.DAS.AssessorService.Web.ViewModels.Organisation;
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
                cfg.CreateMap<OrganisationSearchViewModel, RequestAccessOrgSearchViewModel>()
                .ForMember(dest => dest.Address, x => x.Ignore())
                .ForMember(dest => dest.CompanyNumber, x => x.Ignore())
                .ForMember(dest => dest.CompanyOrCharityDisplayText, x => x.Ignore())
                .ForMember(dest => dest.OrganisationIsLive, x => x.Ignore())
                .ForMember(dest => dest.RoEPAOApproved, x => x.Ignore());
                cfg.CreateMap<ContactResponse, UserViewModel>();
            });
        }
    }
}