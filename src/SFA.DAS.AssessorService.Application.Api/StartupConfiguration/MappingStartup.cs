using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.DomainModels;
using Contact = SFA.DAS.AssessorService.Domain.Entities.Contact;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Application.Api.StartupConfiguration
{
    public static class MappingStartup
    {
        public static void AddMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Organisation, OrganisationResponse>();
                cfg.CreateMap<CreateOrganisationRequest, CreateOrganisationDomainModel>();
                cfg.CreateMap<CreateOrganisationDomainModel, Organisation>();
                cfg.CreateMap<UpdateOrganisationRequest, UpdateOrganisationDomainModel>();
                cfg.CreateMap<CreateContactRequest, CreateContactDomainModel>();
                cfg.CreateMap<CreateContactDomainModel, Contact>();
                cfg.CreateMap<Contact, CreateContactRequest>();
                cfg.CreateMap<Contact, ContactResponse>();
                cfg.CreateMap<Organisation, OrganisationDomainModel>();
                cfg.CreateMap<OrganisationDomainModel, UpdateOrganisationDomainModel>();
            });
        }
    }
}