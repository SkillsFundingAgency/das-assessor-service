using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.AutoMapperExtensions;
using SFA.DAS.AssessorService.Web.AutoMapperProfiles;
using SFA.DAS.AssessorService.Web.Controllers.ManageUsers.ViewModels;
using SFA.DAS.AssessorService.Web.ViewModels.Organisation;
using SFA.DAS.AssessorService.Web.ViewModels.Search;
using SFA.DAS.AssessorService.Web.ViewModels.Shared;
using System.Reflection;

namespace SFA.DAS.AssessorService.Web.StartupConfiguration
{
    public static class MappingStartup
    {
        public static void AddMappings(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.CreateMap<ResultViewModel, SearchResult>()
                    .IgnoreUnmappedMembers();

                cfg.CreateMap<StandardVersionViewModel, StandardVersion>()
                    .IgnoreUnmappedMembers()
                    .ReverseMap();
                
                cfg.CreateMap<OrganisationSearchViewModel, RequestAccessOrgSearchViewModel>()
                    .IgnoreUnmappedMembers()
                    .ForMember(dest => dest.Address, x => x.Ignore())
                    .ForMember(dest => dest.CompanyNumber, x => x.Ignore())
                    .ForMember(dest => dest.CompanyOrCharityDisplayText, x => x.Ignore())
                    .ForMember(dest => dest.OrganisationIsLive, x => x.Ignore())
                    .ForMember(dest => dest.RoEPAOApproved, x => x.Ignore());
                
                cfg.CreateMap<ContactResponse, UserViewModel>()
                    .IgnoreUnmappedMembers();

                //cfg.AddProfile<CompaniesHouseSummaryProfile>();
                //cfg.AddProfile<DirectorInformationProfile>();
                //cfg.AddProfile<PersonSignificantControlInformationProfile>();
                //cfg.AddProfile<CharityCommissionSummaryProfile>();
                //cfg.AddProfile<CharityTrusteeProfile>();
                //cfg.AddProfile<RoatpOrganisationProfile>();
            });

            var serviceProvider = services.BuildServiceProvider();

            // Resolve the IMapper instance
            var mapper = serviceProvider.GetRequiredService<IMapper>();
            //mapper.PrintMappings<Organisation, OrganisationResponse>();
            //mapper.PrintMappings<EpaOrganisation, UpdateEpaOrganisationRequest>();
            //mapper.PrintMappings<ApplicationListItem, ApplicationSummaryItem>();

            string referencingAssemblyPath = Assembly.GetExecutingAssembly().Location;
            MapCallAnalyzer.FindAndPrintMapCalls(referencingAssemblyPath, mapper.ConfigurationProvider);
        }
    }
}