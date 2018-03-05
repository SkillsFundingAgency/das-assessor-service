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
                cfg.CreateMap<Organisation, AssessorService.Api.Types.Models.Organisation>();
                cfg.CreateMap<CreateOrganisationRequest, OrganisationCreateDomainModel>();
                cfg.CreateMap<OrganisationCreateDomainModel, Organisation>();
                cfg.CreateMap<UpdateOrganisationRequest, OrganisationUpdateDomainModel>();
                cfg.CreateMap<CreateContactRequest, ContactCreateDomainModel>();
                cfg.CreateMap<ContactCreateDomainModel, Contact>();
                cfg.CreateMap<Contact, CreateContactRequest>();
                cfg.CreateMap<Contact, AssessorService.Api.Types.Models.Contact>();
                cfg.CreateMap<Organisation, OrganisationQueryDomainModel>();
                cfg.CreateMap<OrganisationQueryDomainModel, OrganisationUpdateDomainModel>();
            });
        }
    }
}