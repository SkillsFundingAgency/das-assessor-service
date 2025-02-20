using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.AutoMapperExtensions;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp.Types;
using System;

namespace SFA.DAS.AssessorService.Application.Api.AutoMapperProfiles
{
    public class RoatpOrganisationProfile : ExplicitMappingProfileBase
    {
        public RoatpOrganisationProfile()
        {
            CreateMap<Organisation, OrganisationSearchResult>()
                .BeforeMap((source, dest) => dest.OrganisationReferenceType = "RoATP")
                .BeforeMap((source, dest) => dest.OrganisationType = "Training Provider")
                .BeforeMap((source, dest) => dest.RoATPApproved = true)
                .BeforeMap((source, dest) => dest.EasApiOrganisationType = null)
                .BeforeMap((source, dest) => dest.Email = null)
                .BeforeMap((source, dest) => dest.Address = null)
                .ForMember(dest => dest.Ukprn, opt => opt.MapFrom(source => source.UKPRN))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => source.UKPRN))
                .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(source => source.LegalName))
                .ForMember(dest => dest.TradingName, opt => opt.ResolveUsing(source => string.IsNullOrEmpty(source.TradingName) || "NULL".Equals(source.TradingName, StringComparison.InvariantCultureIgnoreCase) ? null : source.TradingName))
                .ForMember(dest => dest.CompanyNumber, opt => opt.ResolveUsing(source => source.OrganisationData?.CompanyNumber))
                .ForMember(dest => dest.CharityNumber, opt => opt.ResolveUsing(source => source.OrganisationData?.CharityNumber));
        }
    }
}
