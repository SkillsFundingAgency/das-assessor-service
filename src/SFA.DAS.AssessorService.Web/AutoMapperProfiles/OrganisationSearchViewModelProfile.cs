using System;
using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.AutoMapperExtensions;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp.Types;
using SFA.DAS.AssessorService.Web.ViewModels.Organisation;

namespace SFA.DAS.AssessorService.Web.AutoMapperProfiles
{
    public class OrganisationSearchViewModelProfile : Profile
    {
        public OrganisationSearchViewModelProfile()
        {
            CreateMap<OrganisationSearchViewModel, RequestAccessOrgSearchViewModel>()
                .IgnoreUnmappedMembers()
                .ForMember(dest => dest.Address, x => x.Ignore())
                .ForMember(dest => dest.CompanyNumber, x => x.Ignore())
                .ForMember(dest => dest.CompanyOrCharityDisplayText, x => x.Ignore())
                .ForMember(dest => dest.OrganisationIsLive, x => x.Ignore())
                .ForMember(dest => dest.RoEPAOApproved, x => x.Ignore());
        }
    }
}