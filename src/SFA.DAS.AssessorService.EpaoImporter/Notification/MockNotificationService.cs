using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
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

        public Task Send(int batchNumber, List<CertificateResponse> certificates)
        {
            _logger.LogInfo("Sending Notification ..");
            return Task.CompletedTask;
        }
    }
}
