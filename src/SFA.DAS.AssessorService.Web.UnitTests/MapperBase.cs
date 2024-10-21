using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Web.AutoMapperProfiles;
using SFA.DAS.AssessorService.Web.Controllers.ManageUsers.ViewModels;
using SFA.DAS.AssessorService.Web.ViewModels.Organisation;
using SFA.DAS.AssessorService.Web.ViewModels.Search;
using SFA.DAS.AssessorService.Web.ViewModels.Shared;

namespace SFA.DAS.AssessorService.Application.Web.UnitTests
{
    public class MapperBase
    {
        public IMapper Mapper { get; private set; }

        public MapperBase()
        {
            var services = new ServiceCollection();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ResultViewModel, SearchResult>();

                cfg.CreateMap<StandardVersionViewModel, StandardVersion>()
                    .ReverseMap();

                cfg.CreateMap<OrganisationSearchViewModel, RequestAccessOrgSearchViewModel>()
                    .ForMember(dest => dest.Address, x => x.Ignore())
                    .ForMember(dest => dest.CompanyNumber, x => x.Ignore())
                    .ForMember(dest => dest.CompanyOrCharityDisplayText, x => x.Ignore())
                    .ForMember(dest => dest.OrganisationIsLive, x => x.Ignore())
                    .ForMember(dest => dest.RoEPAOApproved, x => x.Ignore());

                cfg.CreateMap<ContactResponse, UserViewModel>();

                cfg.AddProfile<CompaniesHouseSummaryProfile>();
                cfg.AddProfile<DirectorInformationProfile>();
                cfg.AddProfile<PersonSignificantControlInformationProfile>();
                cfg.AddProfile<CharityCommissionSummaryProfile>();
                cfg.AddProfile<CharityTrusteeProfile>();
                cfg.AddProfile<RoatpOrganisationProfile>();
            });

            //config.AssertConfigurationIsValid();

            Mapper = config.CreateMapper();
        }
    }
}
