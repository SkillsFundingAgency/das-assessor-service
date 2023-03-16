using SFA.DAS.AssessorService.Application.Api.StartupConfiguration;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests
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

            MappingStartup.AddMappings();
        }
    }
}