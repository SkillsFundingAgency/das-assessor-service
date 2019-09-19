using AutoMapper;
using System;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.AutoMapperProfiles
{
    public class AssessorServiceOrganisationProfile : Profile
    {
        public AssessorServiceOrganisationProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.AO.AssessmentOrganisationSummary, OrganisationSearchResult>()
                .BeforeMap((source, dest) => dest.OrganisationReferenceType = "RoEPAO")
                .BeforeMap((source, dest) => dest.RoEPAOApproved = true)
                .BeforeMap((source, dest) => dest.EasApiOrganisationType = null)
                .ForMember(dest => dest.Ukprn, opt => opt.MapFrom(source => source.Ukprn))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.TradingName, opt => opt.MapFrom(source => source.Name))
                .ForMember(dest => dest.LegalName, opt => opt.ResolveUsing(source => source.OrganisationData?.LegalName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(source => source.Email))
                .ForMember(dest => dest.OrganisationType, opt => opt.MapFrom(source => source.OrganisationType))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(source => Mapper.Map< AssessorService.Api.Types.Models.AO.OrganisationData, OrganisationAddress>(source.OrganisationData)))
                .ForMember(dest => dest.CompanyNumber, opt => opt.ResolveUsing(source => source.OrganisationData?.CompanyNumber))
                .ForMember(dest => dest.CharityNumber, opt => opt.ResolveUsing(source => source.OrganisationData?.CharityNumber))
                .ForMember(dest => dest.FinancialDueDate, opt => opt.ResolveUsing(source => source.OrganisationData?.FHADetails?.FinancialDueDate))
                .ForMember(dest => dest.FinancialExempt, opt => opt.ResolveUsing(source => source.OrganisationData?.FHADetails?.FinancialExempt))
                .ForMember(dest => dest.OrganisationIsLive, opt => opt.ResolveUsing(source => source.Status.Equals("Live", StringComparison.CurrentCultureIgnoreCase) ? true : false))
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }

    public class AssessorServiceOrganisationAddressProfile : Profile
    {
        public AssessorServiceOrganisationAddressProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.AO.OrganisationData, OrganisationAddress>()
                .ForMember(dest => dest.Address1, opt => opt.MapFrom(source => source.Address1))
                .ForMember(dest => dest.Address2, opt => opt.MapFrom(source => source.Address2))
                .ForMember(dest => dest.Address3, opt => opt.MapFrom(source => source.Address3))
                .ForMember(dest => dest.City, opt => opt.MapFrom(source => source.Address4))
                .ForMember(dest => dest.Postcode, opt => opt.MapFrom(source => source.Postcode))
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
    public class AssessorServiceOrganisationTypeProfile : Profile
    {
        public AssessorServiceOrganisationTypeProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.AO.OrganisationType, OrganisationType>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(source => source.Type))
                .ForMember(dest => dest.TypeDescription, opt => opt.MapFrom(source => source.TypeDescription))
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }

    public class AssessorServiceOrganisationResponse : Profile
    {
        public AssessorServiceOrganisationResponse()
        {
            CreateMap<Domain.Entities.Organisation,OrganisationResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom( source => source.Id))
                .ForMember(dest => dest.PrimaryContact, opt => opt.MapFrom(source => source.PrimaryContact))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(source => source.Status))
                .ForMember(dest => dest.EndPointAssessorName, opt => opt.MapFrom(source => source.EndPointAssessorName))
                .ForMember(dest => dest.EndPointAssessorOrganisationId, opt => opt.MapFrom(source => source.EndPointAssessorOrganisationId))
                .ForMember(dest => dest.EndPointAssessorUkprn, opt => opt.MapFrom(source => source.EndPointAssessorUkprn))
                .ForMember(dest => dest.RoATPApproved, opt => opt.ResolveUsing(source => source.OrganisationData?.RoATPApproved))
                .ForMember(dest => dest.RoEPAOApproved, opt => opt.ResolveUsing(source => source.OrganisationData?.RoEPAOApproved))
                .ForMember(dest => dest.CompanySummary, opt => opt.ResolveUsing(source => source.OrganisationData?.CompanySummary))
                .ForMember(dest => dest.CharitySummary, opt => opt.ResolveUsing(source => source.OrganisationData?.CharitySummary));
        }
    }
}
