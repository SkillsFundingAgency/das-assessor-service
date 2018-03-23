using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Data.UnitTests
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
                cfg.CreateMap<Domain.Entities.Organisation, OrganisationResponse>();
                cfg.CreateMap<Domain.Entities.Contact, CreateContactRequest>();
                cfg.CreateMap<Domain.Entities.Contact, ContactResponse>();
            });
        }
    }
}
