namespace SFA.DAS.AssessorService.Application.Api.UnitTests
{
    using SFA.DAS.AssessorService.Domain.Entities;
    using SFA.DAS.AssessorService.ViewModel.Models;

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
                cfg.CreateMap<Domain.Entities.Organisation, ViewModel.Models.Organisation>();
                cfg.CreateMap<CreateOrganisationRequest, OrganisationCreateDomainModel>();
                cfg.CreateMap<OrganisationCreateDomainModel, Domain.Entities.Organisation>();
                cfg.CreateMap<Domain.Entities.Organisation, ViewModel.Models.Organisation>();

                cfg.CreateMap<UpdateOrganisationRequest, OrganisationUpdateDomainModel>();
                cfg.CreateMap<Domain.Entities.Organisation, ViewModel.Models.Organisation>();
                cfg.CreateMap<CreateContactRequest, ContactCreateDomainModel>();
                cfg.CreateMap<ContactCreateDomainModel, Domain.Entities.Contact>();
                cfg.CreateMap<Domain.Entities.Contact, CreateContactRequest>();
                cfg.CreateMap<Domain.Entities.Contact, ViewModel.Models.Contact>();
            });
        }
    }
}
