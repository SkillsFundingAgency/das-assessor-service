using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.ViewModel.Models;

namespace SFA.DAS.AssessorService.Application.Api.StartupConfiguration
{
    public static class MappingStartup
    {
        public static void AddMappings()
        {
            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Organisation, OrganisationQueryViewModel>();
                cfg.CreateMap<OrganisationCreateViewModel, OrganisationCreateDomainModel>();
                cfg.CreateMap<OrganisationCreateDomainModel, Organisation>();
                cfg.CreateMap<OrganisationUpdateViewModel, OrganisationUpdateDomainModel>();
                cfg.CreateMap<ContactCreateViewModel, ContactCreateDomainModel>();
                cfg.CreateMap<ContactCreateDomainModel, Contact>();
                cfg.CreateMap<Contact, ContactCreateViewModel>();
                cfg.CreateMap<Contact, ContactQueryViewModel>();
            });
        }
    }
}