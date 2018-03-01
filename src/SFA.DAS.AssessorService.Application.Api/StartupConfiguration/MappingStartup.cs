namespace SFA.DAS.AssessorService.Application.Api.StartupConfiguration
{
    using AssessorService.Api.Types.Models;
    using Domain;

    public static class MappingStartup
    {
        public static void AddMappings()
        {
            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<AssessorService.Domain.Entities.Organisation, Organisation>();
                cfg.CreateMap<CreateOrganisationRequest, OrganisationCreateDomainModel>();
                cfg.CreateMap<OrganisationCreateDomainModel, AssessorService.Domain.Entities.Organisation>();
                cfg.CreateMap<UpdateOrganisationRequest, OrganisationUpdateDomainModel>();
                cfg.CreateMap<CreateContactRequest, ContactCreateDomainModel>();
                cfg.CreateMap<ContactCreateDomainModel, AssessorService.Domain.Entities.Contact>();
                cfg.CreateMap<AssessorService.Domain.Entities.Contact, CreateContactRequest>();
                cfg.CreateMap<AssessorService.Domain.Entities.Contact, Contact>();
                cfg.CreateMap<AssessorService.Domain.Entities.Organisation, OrganisationQueryDomainModel>();
                cfg.CreateMap<OrganisationQueryDomainModel, OrganisationUpdateDomainModel>();
            });
        }
    }
}