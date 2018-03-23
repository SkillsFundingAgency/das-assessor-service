using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.DomainModels;

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
                cfg.CreateMap<AssessorService.Domain.Entities.Organisation, OrganisationResponse>();
                cfg.CreateMap<CreateOrganisationRequest, CreateOrganisationDomainModel>();
                cfg.CreateMap<CreateOrganisationDomainModel, AssessorService.Domain.Entities.Organisation>();
                cfg.CreateMap<AssessorService.Domain.Entities.Organisation, OrganisationResponse>();

                cfg.CreateMap<UpdateOrganisationRequest, UpdateOrganisationDomainModel>();
                cfg.CreateMap<AssessorService.Domain.Entities.Organisation, OrganisationResponse>();
                cfg.CreateMap<CreateContactRequest, CreateContactDomainModel>();
                cfg.CreateMap<CreateContactDomainModel, AssessorService.Domain.Entities.Contact>();
                cfg.CreateMap<AssessorService.Domain.Entities.Contact, CreateContactRequest>();
                cfg.CreateMap<AssessorService.Domain.Entities.Contact, ContactResponse>();
                cfg.CreateMap<AssessorService.Domain.Entities.Contact, OrganisationDomainModel>();
                cfg.CreateMap<OrganisationDomainModel, UpdateOrganisationDomainModel>();
            });
        }
    }
}
