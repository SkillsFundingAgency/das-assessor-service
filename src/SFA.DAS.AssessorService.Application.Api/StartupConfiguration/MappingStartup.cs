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
                cfg.CreateMap<Domain.Entities.Organisation, AssessorService.Api.Types.Organisation>();
                cfg.CreateMap<CreateOrganisationRequest, OrganisationCreateDomainModel>();
                cfg.CreateMap<OrganisationCreateDomainModel, Domain.Entities.Organisation>();
                cfg.CreateMap<UpdateOrganisationRequest, OrganisationUpdateDomainModel>();
                cfg.CreateMap<CreateContactRequest, ContactCreateDomainModel>();
                cfg.CreateMap<ContactCreateDomainModel, Domain.Entities.Contact>();
                cfg.CreateMap<Domain.Entities.Contact, CreateContactRequest>();
                cfg.CreateMap<Domain.Entities.Contact, ViewModel.Models.Contact>();
            });
        }
    }
}