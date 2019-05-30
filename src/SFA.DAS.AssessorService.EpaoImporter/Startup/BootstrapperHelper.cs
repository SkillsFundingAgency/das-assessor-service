using SFA.DAS.AssessorService.EpaoImporter.InfrastructureServices;

namespace SFA.DAS.AssessorService.EpaoImporter.Startup
{
    public static class BootstrapperHelper
    {
        private static bool IsStarted = false;
        private static object _syncLock = new object();
        
        public static void StartUp()
        {
            if (!IsStarted)
            {
                lock (_syncLock)
                {
                    if (!IsStarted)
                    {
                        AssemblyBindingRedirectHelper.ConfigureBindingRedirects();
                        IsStarted = true;
                    }
                }
            }
        }
    }
}
