using SFA.DAS.AssessorService.EpaoImporter.InfrastructureServices;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.EpaoImporter
{ 
    public class ConfigurationWrapper: IConfigurationWrapper
    {
        public IWebConfiguration GetConfiguration()
        {
            return ConfigurationHelper.GetConfiguration();
        }
    }

    public interface IConfigurationWrapper
    {
        IWebConfiguration GetConfiguration();
    }
}
