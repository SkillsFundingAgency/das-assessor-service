namespace SFA.DAS.AssessorService.Application.Api.UnitTests
{
    using AssessorService.Api.Types.Models;
    using Domain;
 
    class Bootstrapper
    {
        public static void Initialize()
        {
            SetupAutomapper();
        }

        public static void SetupAutomapper()
        {
            AutoMapper.Mapper.Reset();

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<AssessorService.Domain.Entities.Organisation, Organisation>();
                cfg.CreateMap<CreateOrganisationRequest, OrganisationCreateDomainModel>();
                cfg.CreateMap<OrganisationCreateDomainModel, AssessorService.Domain.Entities.Organisation>();
                cfg.CreateMap<AssessorService.Domain.Entities.Organisation, Organisation>();

                cfg.CreateMap<UpdateOrganisationRequest, OrganisationUpdateDomainModel>();
                cfg.CreateMap<AssessorService.Domain.Entities.Organisation, Organisation>();
                cfg.CreateMap<CreateContactRequest, ContactCreateDomainModel>();
                cfg.CreateMap<ContactCreateDomainModel, AssessorService.Domain.Entities.Contact>();
                cfg.CreateMap<AssessorService.Domain.Entities.Contact, CreateContactRequest>();
                cfg.CreateMap<AssessorService.Domain.Entities.Contact, Contact>();
            });
        }
    }
}
