using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.DomainModels;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests
{
    class MappingBootstrapper
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
                cfg.CreateMap<Organisation, OrganisationResponse>();
                cfg.CreateMap<CreateOrganisationRequest, CreateOrganisationDomainModel>();
                cfg.CreateMap<CreateOrganisationDomainModel, Organisation>();
                cfg.CreateMap<Organisation, OrganisationResponse>();

                cfg.CreateMap<UpdateOrganisationRequest, UpdateOrganisationDomainModel>();
                cfg.CreateMap<Organisation, OrganisationResponse>();
                cfg.CreateMap<CreateContactRequest, Contact>();
                cfg.CreateMap<CreateContactDomainModel, Contact>();
                cfg.CreateMap<Contact, CreateContactRequest>();
                cfg.CreateMap<Contact, ContactResponse>();
                cfg.CreateMap<Contact, OrganisationDomainModel>();
                cfg.CreateMap<OrganisationDomainModel, UpdateOrganisationDomainModel>();
            });
        }
    }
}
