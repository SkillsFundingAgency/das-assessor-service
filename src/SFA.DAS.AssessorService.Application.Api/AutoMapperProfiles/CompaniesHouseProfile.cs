using AutoMapper;
using System;
using System.Linq;
using SFA.DAS.AssessorService.Api.Types.Models.CompaniesHouse;
using SFA.DAS.AssessorService.AutoMapperExtensions;

namespace SFA.DAS.AssessorService.Application.Api.AutoMapperProfiles
{
    public class CompaniesHouseCompanyProfile : ExplicitMappingProfileBase    
    {
        public CompaniesHouseCompanyProfile()
        {
            CreateMap<CompanyDetails, AssessorService.Api.Types.CompaniesHouse.Company>()
                .ForMember(dest => dest.CompanyNumber, opt => opt.MapFrom(source => source.company_number))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(source => source.company_name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(source => source.company_status))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(source => source.type))
                .ForMember(dest => dest.NatureOfBusiness, opt => opt.MapFrom(source => source.sic_codes))
                .ForMember(dest => dest.IncorporatedOn, opt => opt.MapFrom(source => source.date_of_creation))
                .ForMember(dest => dest.DissolvedOn, opt => opt.MapFrom(source => source.date_of_cessation))
                .ForMember(dest => dest.IsLiquidated, opt => opt.MapFrom(source => source.has_been_liquidated))
                .ForMember(dest => dest.PreviousNames, opt => opt.ResolveUsing(source => source.previous_company_names?.Select(pc => pc.name)))
                .ForMember(dest => dest.RegisteredOfficeAddress, opt => opt.MapFrom(source => source.registered_office_address)) 
                .ForMember(dest => dest.Accounts, opt => opt.MapFrom(source => source));
        }
    }

    public class CompaniesHouseAccountsProfile : ExplicitMappingProfileBase
    {
        public CompaniesHouseAccountsProfile()
        {
            CreateMap<CompanyDetails, AssessorService.Api.Types.CompaniesHouse.Accounts>()
                .ForMember(dest => dest.LastConfirmationStatementDate, opt => opt.ResolveUsing(source => source.confirmation_statement?.last_made_up_to))
                .ForMember(dest => dest.LastAccountsDate, opt => opt.ResolveUsing(source => source.accounts?.last_accounts?.made_up_to));
        }
    }

    public class CompaniesHouseRegisteredOfficeAddressProfile : ExplicitMappingProfileBase
    {
        public CompaniesHouseRegisteredOfficeAddressProfile()
        {
            CreateMap<RegisteredOfficeAddress, AssessorService.Api.Types.CompaniesHouse.Address>()
                .ForMember(dest => dest.AddressLine1, opt => opt.ResolveUsing(source => $"{source.po_box} {source.premises} {source.address_line_1}".TrimStart()))
                .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(source => source.address_line_2))
                .ForMember(dest => dest.City, opt => opt.MapFrom(source => source.locality))
                .ForMember(dest => dest.County, opt => opt.MapFrom(source => source.region))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(source => source.country))
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(source => source.postal_code));
        }
    }

    public class CompaniesHouseOfficerAddressProfile : ExplicitMappingProfileBase
    {
        public CompaniesHouseOfficerAddressProfile()
        {
            CreateMap<OfficerAddress, AssessorService.Api.Types.CompaniesHouse.Address>()
                .ForMember(dest => dest.AddressLine1, opt => opt.ResolveUsing(source => $"{source.po_box} {source.premises} {source.address_line_1}".TrimStart()))
                .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(source => source.address_line_2))
                .ForMember(dest => dest.City, opt => opt.MapFrom(source => source.locality))
                .ForMember(dest => dest.County, opt => opt.MapFrom(source => source.region))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(source => source.country))
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(source => source.postal_code));
        }
    }

    public class CompaniesHouseOfficerProfile : ExplicitMappingProfileBase
    {
        public CompaniesHouseOfficerProfile()
        {
            CreateMap<Officer, AssessorService.Api.Types.CompaniesHouse.Officer>()
                .ForMember(dest => dest.Id, opt => opt.ResolveUsing(source => source.links?.officer?.appointments?.Replace("/officers/", string.Empty).Replace("/appointments", string.Empty)))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(source => source.name))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(source => source.officer_role))
                .ForMember(dest => dest.DateOfBirth, opt => opt.ResolveUsing(source => source.date_of_birth is null ? DateTime.MinValue : new DateTime(source.date_of_birth.year, source.date_of_birth.month, source.date_of_birth.day ?? 1)))
                .ForMember(dest => dest.AppointedOn, opt => opt.MapFrom(source => source.appointed_on))
                .ForMember(dest => dest.ResignedOn, opt => opt.MapFrom(source => source.resigned_on))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(source => source.address));
        }
    }

    public class CompaniesHouseOfficerDisqualificationProfile : ExplicitMappingProfileBase
    {
        public CompaniesHouseOfficerDisqualificationProfile()
        {
            CreateMap<Disqualification, AssessorService.Api.Types.CompaniesHouse.Disqualification>()
                .ForMember(dest => dest.DisqualifiedFrom, opt => opt.MapFrom(source => source.disqualified_from))
                .ForMember(dest => dest.DisqualifiedUntil, opt => opt.MapFrom(source => source.disqualified_until))
                .ForMember(dest => dest.CaseIdentifier, opt => opt.MapFrom(source => source.case_identifier))
                .ForMember(dest => dest.Reason, opt => opt.ResolveUsing(source => source.reason?.act))
                .ForMember(dest => dest.ReasonDescription, opt => opt.ResolveUsing(source => source.reason?.description_identifier));
        }
    }

    public class CompaniesHousePersonWithSignificantControlProfile : ExplicitMappingProfileBase
    {
        public CompaniesHousePersonWithSignificantControlProfile()
        {
            CreateMap<PersonWithSignificantControl, AssessorService.Api.Types.CompaniesHouse.PersonWithSignificantControl>()
                .ForMember(dest => dest.Id, opt => opt.ResolveUsing(source => source.links?.self))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(source => source.name))
                .ForMember(dest => dest.DateOfBirth, opt => opt.ResolveUsing(source => source.date_of_birth is null ? DateTime.MinValue : new DateTime(source.date_of_birth.year, source.date_of_birth.month, source.date_of_birth.day ?? 1)))
                .ForMember(dest => dest.NaturesOfControl, opt => opt.MapFrom(source => source.natures_of_control))
                .ForMember(dest => dest.NotifiedOn, opt => opt.MapFrom(source => source.notified_on))
                .ForMember(dest => dest.CeasedOn, opt => opt.MapFrom(source => source.ceased_on))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(source => source.address));
        }
    }

    public class CompaniesHousePersonWithSignificantControlAddressProfile : ExplicitMappingProfileBase
    {
        public CompaniesHousePersonWithSignificantControlAddressProfile()
        {
            CreateMap<PersonWithSignificantControlAddress, AssessorService.Api.Types.CompaniesHouse.Address>()
                .ForMember(dest => dest.AddressLine1, opt => opt.ResolveUsing(source => $"{source.po_box} {source.premises} {source.address_line_1}".TrimStart()))
                .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(source => source.address_line_2))
                .ForMember(dest => dest.City, opt => opt.MapFrom(source => source.locality))
                .ForMember(dest => dest.County, opt => opt.MapFrom(source => source.region))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(source => source.country))
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(source => source.postal_code));
        }
    }
}