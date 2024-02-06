using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp.Types;
using System;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.Api.AutoMapperProfiles
{
    public class UkrlpOrganisationProfile : Profile
    {
        public UkrlpOrganisationProfile()
        {
            CreateMap<ProviderDetails, OrganisationSearchResult>()
                .BeforeMap((source, dest) => dest.OrganisationReferenceType = "UKRLP")
                .BeforeMap((source, dest) => dest.OrganisationType = "Training Provider")
                .BeforeMap((source, dest) => dest.EasApiOrganisationType = null)
                .ForMember(dest => dest.Ukprn, opt => opt.MapFrom(source => source.UKPRN))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => source.UKPRN))
                .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(source => source.ProviderName))
                .ForMember(dest => dest.TradingName, opt => opt.ResolveUsing(source => source.ProviderAliases?.FirstOrDefault()?.Alias))
                .ForMember(dest => dest.CompanyNumber, opt => opt.ResolveUsing(source => source.VerificationDetails?.FirstOrDefault(vd => "companies house".Equals(vd.VerificationAuthority, StringComparison.InvariantCultureIgnoreCase))?.VerificationId))
                .ForMember(dest => dest.CharityNumber, opt => opt.ResolveUsing(source => source.VerificationDetails?.FirstOrDefault(vd => "charity commission".Equals(vd.VerificationAuthority, StringComparison.InvariantCultureIgnoreCase))?.VerificationId))
                .ForMember(dest => dest.Email, opt => opt.ResolveUsing(source => source.ContactDetails?.FirstOrDefault(cd => !string.IsNullOrEmpty(cd.ContactEmail))?.ContactEmail))
                .ForMember(dest => dest.Address, opt => opt.ResolveUsing(source => Mapper.Map<ContactAddress, OrganisationAddress>(source.ContactDetails?.Select(cd => cd.ContactAddress).FirstOrDefault(address => address != null))))
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }

    public class UkrlpOrganisationAddressProfile : Profile
    {
        public UkrlpOrganisationAddressProfile()
        {
            CreateMap<ContactAddress, OrganisationAddress>()
                .ForMember(dest => dest.Address1, opt => opt.MapFrom(source => source.Address1))
                .ForMember(dest => dest.Address2, opt => opt.MapFrom(source => source.Address2))
                .ForMember(dest => dest.Address3, opt => opt.MapFrom(source => source.Address3))
                .ForMember(dest => dest.City, opt => opt.MapFrom(source => source.Town))
                .ForMember(dest => dest.Postcode, opt => opt.MapFrom(source => source.PostCode))
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}
