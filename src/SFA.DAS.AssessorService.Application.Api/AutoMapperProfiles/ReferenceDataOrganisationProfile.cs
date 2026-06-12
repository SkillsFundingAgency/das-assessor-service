using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.AutoMapperExtensions;


namespace SFA.DAS.AssessorService.Application.Api.AutoMapperProfiles
{
    public class ReferenceDataOrganisationProfile : ExplicitMappingProfileBase
    {
        public ReferenceDataOrganisationProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.ReferenceData.Organisation,OrganisationSearchResult>()
                .BeforeMap((source, dest) => dest.Ukprn = null)
                .BeforeMap((source, dest) => dest.OrganisationReferenceType = "EASAPI")
                .BeforeMap((source, dest) => dest.Email = null)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => source.Code))
                .ForMember(dest => dest.LegalName, opt => opt.MapFrom(source => source.Name))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(source => source.Address))
                .ForMember(dest => dest.CompanyNumber, opt => opt.ResolveUsing(source => source.Type == AssessorService.Api.Types.Models.ReferenceData.OrganisationType.Company ? source.Code : null))
                .ForMember(dest => dest.CharityNumber, opt => opt.ResolveUsing(source => source.Type == AssessorService.Api.Types.Models.ReferenceData.OrganisationType.Charity ? source.Code : null))
                .ForMember(dest => dest.EasApiOrganisationType, opt => opt.MapFrom(source => source.Type.ToString()));
        }
    }

    public class ReferenceDataOrganisationAddressProfile : ExplicitMappingProfileBase
    {
        public ReferenceDataOrganisationAddressProfile()
        {
            CreateMap<SFA.DAS.AssessorService.Api.Types.Models.ReferenceData.Address, OrganisationAddress>()
                .ForMember(dest => dest.Address1, opt => opt.MapFrom(source => source.Line1))
                .ForMember(dest => dest.Address2, opt => opt.MapFrom(source => source.Line2))
                .ForMember(dest => dest.Address3, opt => opt.MapFrom(source => source.Line3))
                .ForMember(dest => dest.City, opt => opt.MapFrom(source => source.Line4))
                .ForMember(dest => dest.Postcode, opt => opt.MapFrom(source => source.Postcode));
        }
    }
}
