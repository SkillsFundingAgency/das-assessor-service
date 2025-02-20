using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.AutoMapperExtensions;

namespace SFA.DAS.AssessorService.Application.Mapping.AutoMapperProfiles
{
    public class EpaOrganisationProfile : ExplicitMappingProfileBase
    {
        public EpaOrganisationProfile()
        {
            // Request going to Int API
            CreateMap<EpaOrganisation, UpdateEpaOrganisationRequest>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(source => source.Name))
                .ForMember(dest => dest.OrganisationId, opt => opt.MapFrom(source => source.OrganisationId))
                .ForMember(dest => dest.Ukprn, opt => opt.MapFrom(source => source.Ukprn))
                .ForMember(dest => dest.OrganisationTypeId, opt => opt.MapFrom(source => source.OrganisationTypeId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(source => source.Status))
                .ForMember(dest => dest.LegalName, opt => opt.MapFrom(source => source.OrganisationData.LegalName))
                .ForMember(dest => dest.TradingName, opt => opt.MapFrom(source => source.OrganisationData.TradingName))
                .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(source => source.OrganisationData.ProviderName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(source => source.OrganisationData.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(source => source.OrganisationData.PhoneNumber))
                .ForMember(dest => dest.WebsiteLink, opt => opt.MapFrom(source => source.OrganisationData.WebsiteLink))
                .ForMember(dest => dest.Address1, opt => opt.MapFrom(source => source.OrganisationData.Address1))
                .ForMember(dest => dest.Address2, opt => opt.MapFrom(source => source.OrganisationData.Address2))
                .ForMember(dest => dest.Address3, opt => opt.MapFrom(source => source.OrganisationData.Address3))
                .ForMember(dest => dest.Address4, opt => opt.MapFrom(source => source.OrganisationData.Address4))
                .ForMember(dest => dest.Address4, opt => opt.MapFrom(source => source.OrganisationData.Address4))
                .ForMember(dest => dest.Postcode, opt => opt.MapFrom(source => source.OrganisationData.Postcode))
                .ForMember(dest => dest.CompanyNumber, opt => opt.MapFrom(source => source.OrganisationData.CompanyNumber))
                .ForMember(dest => dest.CompanySummary, opt => opt.MapFrom(source => source.OrganisationData.CompanySummary))
                .ForMember(dest => dest.CharityNumber, opt => opt.MapFrom(source => source.OrganisationData.CharityNumber))
                .ForMember(dest => dest.CharitySummary, opt => opt.MapFrom(source => source.OrganisationData.CharitySummary))
                .ForMember(dest => dest.FinancialDueDate, opt => opt.ResolveUsing(source => source.OrganisationData.FHADetails?.FinancialDueDate))
                .ForMember(dest => dest.FinancialExempt, opt => opt.ResolveUsing(source => source.OrganisationData.FHADetails?.FinancialExempt));
        }
    }
}
