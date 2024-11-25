using AutoMapper;
using SFA.DAS.AssessorService.AutoMapperExtensions;

namespace SFA.DAS.AssessorService.Application.Api.AutoMapperProfiles
{
    public class CharityCommissionProfile : Profile
    {
        public CharityCommissionProfile()
        {
            CreateMap<CharityCommissionService.Charity, AssessorService.Api.Types.CharityCommission.Charity>()
                .IgnoreUnmappedMembers()
                .ForMember(dest => dest.CharityNumber, opt => opt.MapFrom(source => source.RegisteredCharityNumber))
                .ForMember(dest => dest.CompanyNumber, opt => opt.MapFrom(source => source.RegisteredCompanyNumber))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(source => source.CharityName.Trim()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(source => source.Status))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(source => source.Type))
                .ForMember(dest => dest.NatureOfBusiness, opt => opt.MapFrom(source => source.NatureOfBusiness))
                .ForMember(dest => dest.IncorporatedOn, opt => opt.MapFrom(source => source.RegistrationDate))
                .ForMember(dest => dest.DissolvedOn, opt => opt.MapFrom(source => source.RegistrationRemovalDate))
                .ForMember(dest => dest.RegisteredOfficeAddress, opt => opt.MapFrom(source => source.Address))
                .ForMember(dest => dest.Accounts, opt => opt.MapFrom(source => source.LatestFiling))
                .ForMember(dest => dest.Trustees, opt => opt.MapFrom(source => source.Trustees));
        }
    }

    public class CharityCommissionAddressProfile : Profile
    {
        public CharityCommissionAddressProfile()
        {
            CreateMap<CharityCommissionService.Address, AssessorService.Api.Types.CharityCommission.Address>()
                .IgnoreUnmappedMembers()
                .BeforeMap((source, dest) => dest.Country = "United Kingdom")
                .ForMember(dest => dest.AddressLine1, opt => opt.MapFrom(source => source.Line1))
                .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(source => string.IsNullOrEmpty(source.Line3) ? null : source.Line2)) // sometimes city is on line 2
                .ForMember(dest => dest.City, opt => opt.MapFrom(source => source.Line3 ?? source.Line2)) // cope for when it is on line 2, instead of line 3
                .ForMember(dest => dest.County, opt => opt.MapFrom(source => source.Line4)) // not sure what line 4 is
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(source => source.Postcode));
        }
    }

    public class CharityCommissionAccountsProfile : Profile
    {
        public CharityCommissionAccountsProfile()
        {
            CreateMap<CharityCommissionService.LatestFiling, AssessorService.Api.Types.CharityCommission.Accounts>()
                .IgnoreUnmappedMembers()
                .ForMember(dest => dest.LastAccountsDate, opt => opt.ResolveUsing(source => source.AccountsPeriodDateTime > source.AnnualReturnPeriodDateTime ? source.AccountsPeriodDateTime : source.AnnualReturnPeriodDateTime));
        }
    }

    public class CharityCommissionTrusteeProfile : Profile
    {
        public CharityCommissionTrusteeProfile()
        {
            CreateMap<CharityCommissionService.Trustee, AssessorService.Api.Types.CharityCommission.Trustee>()
                .IgnoreUnmappedMembers()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => source.TrusteeNumber))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(source => source.TrusteeName));
        }
    }
}
