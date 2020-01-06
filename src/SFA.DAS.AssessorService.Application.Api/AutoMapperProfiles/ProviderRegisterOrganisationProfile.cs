using AutoMapper;
using System.Linq;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.AutoMapperProfiles
{
    public class ProviderRegisterOrganisationProfile : Profile
    {
        public ProviderRegisterOrganisationProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.ProviderRegister.Provider, OrganisationSearchResult>()
                .BeforeMap((source, dest) => dest.OrganisationReferenceType = "RoATP")
                .BeforeMap((source, dest) => dest.OrganisationType = "Training Provider")
                .BeforeMap((source, dest) => dest.RoATPApproved = true)
                .BeforeMap((source, dest) => dest.EasApiOrganisationType = null)
                .ForMember(dest => dest.Ukprn, opt => opt.MapFrom(source => source.Ukprn))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => source.Ukprn))
                .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(source => source.ProviderName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(source => source.Email))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(source =>
                    Mapper.Map<AssessorService.Api.Types.Models.ProviderRegister.Address, OrganisationAddress>(source.Addresses.FirstOrDefault())))
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }

    public class ProviderRegisterOrganisationAddressProfile : Profile
    {
        public ProviderRegisterOrganisationAddressProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.ProviderRegister.Address, OrganisationAddress>()
                .ForMember(dest => dest.Address1, opt => opt.MapFrom(source => source.Street))
                .ForMember(dest => dest.City, opt => opt.MapFrom(source => source.Town))
                .ForMember(dest => dest.Postcode, opt => opt.MapFrom(source => source.PostCode))
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}
