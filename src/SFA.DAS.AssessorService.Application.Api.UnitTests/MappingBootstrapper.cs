using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests
{
    using AssessorService.Api.Types.Models;

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
                cfg.CreateMap<CreateOrganisationRequest, Organisation>();
                cfg.CreateMap<Organisation, OrganisationResponse>();
                cfg.CreateMap<Contact, CreateContactRequest>();
                cfg.CreateMap<Contact, ContactResponse>();
            });
        }
    }
}