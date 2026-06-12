using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.CompaniesHouse;
using SFA.DAS.AssessorService.AutoMapperExtensions;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.AutoMapperProfiles
{
    public class CompaniesHouseSummaryProfile : ExplicitMappingProfileBase
    {
        public CompaniesHouseSummaryProfile()
        {
            CreateMap<Company, Domain.Entities.CompaniesHouseSummary>()
                .ForMember(dest => dest.CompanyNumber, opt => opt.MapFrom(source => source.CompanyNumber))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(source => source.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(source => source.Status))
                .ForMember(dest => dest.CompanyType, opt => opt.MapFrom(source => source.Type))
                .ForMember(dest => dest.IncorporationDate, opt => opt.MapFrom(source => source.IncorporatedOn))
                .ForMember(dest => dest.Directors, opt => opt.MapFrom(source => source.Officers.Where(x => x.Role.ToLower() == "director")))
                .ForMember(dest => dest.PersonsWithSignificantControl, opt => opt.MapFrom(source => source.PeopleWithSignificantControl))
                .ForMember(dest => dest.CompanyTypeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.ManualEntryRequired, opt => opt.Ignore())
                .AfterMap<MapManualEntryRequiredAction>();
        }

        public class MapManualEntryRequiredAction : IMappingAction<Company, Domain.Entities.CompaniesHouseSummary>
        {
            public void Process(Company source, Domain.Entities.CompaniesHouseSummary destination, ResolutionContext context)
            {
                if (destination != null
                     && (destination.Directors is null || destination.Directors.Count == 0)
                     && (destination.PersonsWithSignificantControl is null || destination.PersonsWithSignificantControl.Count == 0))
                {
                    destination.ManualEntryRequired = true;
                }
            }
        }
    }

    public class DirectorInformationProfile : ExplicitMappingProfileBase
    {
        public DirectorInformationProfile()
        {
            CreateMap<Officer, Domain.Entities.DirectorInformation>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(source => source.Name))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(source => source.DateOfBirth.ToString("MM,yyyy")))
                .ForMember(dest => dest.AppointedDate, opt => opt.MapFrom(source => source.AppointedOn))
                .ForMember(dest => dest.ResignedDate, opt => opt.MapFrom(source => source.ResignedOn));
        }
    }

    public class PersonSignificantControlInformationProfile : ExplicitMappingProfileBase
    {
        public PersonSignificantControlInformationProfile()
        {
            CreateMap<PersonWithSignificantControl, Domain.Entities.PersonWithSignificantControlInformation>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(source => source.Name))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(source => source.DateOfBirth.ToString("MM,yyyy")))
                .ForMember(dest => dest.NotifiedDate, opt => opt.MapFrom(source => source.NotifiedOn))
                .ForMember(dest => dest.CeasedDate, opt => opt.MapFrom(source => source.CeasedOn));
        }
    }
}
