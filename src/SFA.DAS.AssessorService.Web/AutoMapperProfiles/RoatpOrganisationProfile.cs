using System;
using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Web.AutoMapperProfiles
{
    public class RoatpOrganisationProfile : Profile
    {
        public RoatpOrganisationProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.Roatp.Organisation, OrganisationSearchResult>()
                .BeforeMap((source, dest) => dest.OrganisationReferenceType = "RoATP")
                .BeforeMap((source, dest) => dest.OrganisationType = "Training Provider")
                .BeforeMap((source, dest) => dest.RoATPApproved = true)
                .BeforeMap((source, dest) => dest.EasApiOrganisationType = null)
                .BeforeMap((source, dest) => dest.Email = null)
                .BeforeMap((source, dest) => dest.Address = null)
                .ForMember(dest => dest.Ukprn, opt => opt.MapFrom(source => source.UKPRN))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => source.UKPRN))
                .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(source => source.LegalName))
                .ForMember(dest => dest.TradingName, opt => opt.MapFrom(source => string.IsNullOrEmpty(source.TradingName) || "NULL".Equals(source.TradingName, StringComparison.InvariantCultureIgnoreCase) ? null : source.TradingName))
                .ForMember(dest => dest.CompanyNumber, opt => opt.MapFrom(source => (null == source.OrganisationData) ? null : source.OrganisationData.CompanyNumber))
                .ForMember(dest => dest.CharityNumber, opt => opt.MapFrom(source => (null == source.OrganisationData) ? null : source.OrganisationData.CharityNumber))
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}