using System.Threading.Tasks;
using SFA.DAS.AssessorService.EpaoImporter.Logger;

namespace SFA.DAS.AssessorService.EpaoImporter.Notification
{
    public class MockNotificationService : INotificationService
    {
        private readonly IAggregateLogger _logger;

        public MockNotificationService(IAggregateLogger logger)
        {
            _logger = logger;
        }

        public Task Send()
        {
            _logger.LogInfo("Sending Notification ..");
            return Task.CompletedTask;
        }
    }
}
