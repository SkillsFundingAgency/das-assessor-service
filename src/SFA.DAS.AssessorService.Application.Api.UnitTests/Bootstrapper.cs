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
                cfg.CreateMap<Organisation, OrganisationQueryViewModel>();
                cfg.CreateMap<OrganisationCreateViewModel, OrganisationCreateDomainModel>();
                cfg.CreateMap<OrganisationCreateDomainModel, Organisation>();
                cfg.CreateMap<Organisation, OrganisationQueryViewModel>();

                cfg.CreateMap<OrganisationUpdateViewModel, OrganisationUpdateDomainModel>();
                cfg.CreateMap<Organisation, OrganisationQueryViewModel>();
                cfg.CreateMap<ContactCreateViewModel, ContactCreateDomainModel>();
                cfg.CreateMap<ContactCreateDomainModel, Domain.Entities.Contact>();
                cfg.CreateMap<Domain.Entities.Contact, ContactCreateViewModel>();
                cfg.CreateMap<Domain.Entities.Contact, ContactQueryViewModel>();
            });
        }
    }
}
