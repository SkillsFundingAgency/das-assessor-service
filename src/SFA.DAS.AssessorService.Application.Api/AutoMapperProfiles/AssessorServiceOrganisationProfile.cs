using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using OrganisationType = SFA.DAS.AssessorService.Api.Types.Models.OrganisationType;

namespace SFA.DAS.AssessorService.Application.Api.AutoMapperProfiles
{
    public class AssessorServiceOrganisationProfile : Profile
    {
        public AssessorServiceOrganisationProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.AO.AssessmentOrganisationSummary, OrganisationSearchResult>()
                .IgnoreAll()
                .BeforeMap((source, dest) => dest.OrganisationReferenceType = "RoEPAO")
                .BeforeMap((source, dest) => dest.RoEPAOApproved = true)
                .BeforeMap((source, dest) => dest.EasApiOrganisationType = null)
                .ForMember(dest => dest.Ukprn, opt => opt.MapFrom(source => source.Ukprn))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.TradingName, opt => opt.MapFrom(source => source.Name))
                .ForMember(dest => dest.LegalName, opt => opt.ResolveUsing(source => source.OrganisationData?.LegalName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(source => source.Email))
                .ForMember(dest => dest.OrganisationType, opt => opt.MapFrom(source => source.OrganisationType))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(source => source.OrganisationData))
                .ForMember(dest => dest.CompanyNumber, opt => opt.ResolveUsing(source => source.OrganisationData?.CompanyNumber))
                .ForMember(dest => dest.CharityNumber, opt => opt.ResolveUsing(source => source.OrganisationData?.CharityNumber))
                .ForMember(dest => dest.FinancialDueDate, opt => opt.ResolveUsing(source => source.OrganisationData?.FHADetails?.FinancialDueDate))
                .ForMember(dest => dest.FinancialExempt, opt => opt.ResolveUsing(source => source.OrganisationData?.FHADetails?.FinancialExempt))
                .ForMember(dest => dest.OrganisationIsLive, opt => opt.ResolveUsing(source => source.Status.Equals("Live", StringComparison.CurrentCultureIgnoreCase) ? true : false));
        }
    }

    public class AssessorServiceOrganisationAddressProfile : Profile
    {
        public AssessorServiceOrganisationAddressProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.AO.OrganisationData, OrganisationAddress>()
                .IgnoreAll()
                .ForMember(dest => dest.Address1, opt => opt.MapFrom(source => source.Address1))
                .ForMember(dest => dest.Address2, opt => opt.MapFrom(source => source.Address2))
                .ForMember(dest => dest.Address3, opt => opt.MapFrom(source => source.Address3))
                .ForMember(dest => dest.City, opt => opt.MapFrom(source => source.Address4))
                .ForMember(dest => dest.Postcode, opt => opt.MapFrom(source => source.Postcode));
        }
    }
    public class AssessorServiceOrganisationTypeProfile : Profile
    {
        public AssessorServiceOrganisationTypeProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.AO.OrganisationType, OrganisationType>()
                .MapMatchingMembersAndIgnoreOthers();
        }
    }

    public class AssessorServiceOrganisationResponse : Profile
    {
        public AssessorServiceOrganisationResponse()
        {
            CreateMap<Domain.Entities.Organisation, OrganisationResponse>()
                .MapMatchingMembersAndIgnoreOthers()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.PrimaryContact, opt => opt.MapFrom(source => source.PrimaryContact))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(source => source.Status))
                .ForMember(dest => dest.EndPointAssessorName, opt => opt.MapFrom(source => source.EndPointAssessorName))
                .ForMember(dest => dest.EndPointAssessorOrganisationId, opt => opt.MapFrom(source => source.EndPointAssessorOrganisationId))
                .ForMember(dest => dest.EndPointAssessorUkprn, opt => opt.MapFrom(source => source.EndPointAssessorUkprn))
                .ForMember(dest => dest.RoATPApproved, opt => opt.ResolveUsing(source => source.OrganisationData?.RoATPApproved))
                .ForMember(dest => dest.RoEPAOApproved, opt => opt.ResolveUsing(source => source.OrganisationData?.RoEPAOApproved))
                .ForMember(dest => dest.OrganisationType, opt => opt.ResolveUsing(source => source.OrganisationType?.Type))
                .ForMember(dest => dest.CompanySummary, opt => opt.ResolveUsing(source => source.OrganisationData?.CompanySummary))
                .ForMember(dest => dest.CharitySummary, opt => opt.ResolveUsing(source => source.OrganisationData?.CharitySummary));
        }
    }

    public class OrganisationWithStandardResponseMapper : Profile
    {
        public OrganisationWithStandardResponseMapper()
        {
            CreateMap<Domain.Entities.Organisation, OrganisationStandardResponse>()
                .MapMatchingMembersAndIgnoreOthers()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.PrimaryContact, opt => opt.MapFrom(source => source.PrimaryContact))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(source => source.Status))
                .ForMember(dest => dest.EndPointAssessorName, opt => opt.MapFrom(source => source.EndPointAssessorName))
                .ForMember(dest => dest.EndPointAssessorOrganisationId, opt => opt.MapFrom(source => source.EndPointAssessorOrganisationId))
                .ForMember(dest => dest.EndPointAssessorUkprn, opt => opt.MapFrom(source => source.EndPointAssessorUkprn))
                .ForMember(dest => dest.OrganisationType, opt => opt.ResolveUsing(source => source.OrganisationType?.Type))
                .ForMember(dest => dest.City, opt => opt.ResolveUsing(source => source.OrganisationData?.Address4))
                .ForMember(dest => dest.Postcode, opt => opt.ResolveUsing(source => source.OrganisationData?.Postcode))
                .ForMember(dest => dest.DeliveryAreasDetails, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    var firstStandard = src.OrganisationStandards.FirstOrDefault();
                    if (firstStandard != null)
                    {
                        return firstStandard.OrganisationStandardDeliveryAreas
                            .Select(area => context.Mapper.Map<Domain.Entities.OrganisationStandardDeliveryArea, OrganisationStandardDeliveryArea>(area))
                            .ToList();
                    }
                    return new List<OrganisationStandardDeliveryArea>(); 
                }))
                .ForMember(dest =>dest.OrganisationStandard,
                        opt=>opt.MapFrom(src=>src.OrganisationStandards.FirstOrDefault()));
        }
    }

    public class OrganisationStandardDeliveryAreaMapper : Profile
    {
        public OrganisationStandardDeliveryAreaMapper()
        {
            CreateMap<Domain.Entities.OrganisationStandardDeliveryArea, OrganisationStandardDeliveryArea>()
                .IgnoreAll()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.DeliveryArea, opt => opt.MapFrom(source => source.DeliveryArea.Area))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(source => source.Status))
                .ForMember(dest => dest.DeliveryAreaId, opt => opt.MapFrom(source => source.DeliveryArea.Id));
        }
    }

    public class OrganisationStandardMapper : Profile
    {
        public OrganisationStandardMapper ()
        {
            CreateMap<Domain.Entities.OrganisationStandard, OrganisationStandard>()
                .IgnoreAll()
                .ForMember(dest => dest.StandardId, opt=> opt.MapFrom(source =>source.StandardCode))
                .ForMember(dest => dest.EffectiveFrom, opt=> opt.MapFrom(source =>source.EffectiveFrom))
                .ForMember(dest => dest.EffectiveTo, opt=> opt.MapFrom(source =>source.EffectiveTo))
                .ForMember(dest => dest.DateStandardApprovedOnRegister, opt=> opt.MapFrom(source =>source.DateStandardApprovedOnRegister));
        }
    }
}
