using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.DomainServices;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.Notification;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public class PrintProcessFlowCommand : ICommand
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly CoverLetterService _coverLetterService;
        private readonly IFACertificateService _ifaCertificateService;
        private readonly IAssessorServiceApi _assessorServiceApi;
        private readonly INotificationService _notificationService;

        public PrintProcessFlowCommand(IAggregateLogger aggregateLogger,
            CoverLetterService coverLetterService,
            IFACertificateService ifaCertificateService,
            IAssessorServiceApi assessorServiceApi,
            INotificationService notificationService)
        {
            _aggregateLogger = aggregateLogger;
            _coverLetterService = coverLetterService;
            _ifaCertificateService = ifaCertificateService;
            _assessorServiceApi = assessorServiceApi;
            _notificationService = notificationService;
        }

        public async Task Execute()
        {
            try
            {
                _aggregateLogger.LogInfo("Function Started");
                _aggregateLogger.LogInfo("Print Function Flow Started");

                _aggregateLogger.LogInfo("Accessing Environment variables");
                var customSetting =
                    Environment.GetEnvironmentVariable("CustomSetting", EnvironmentVariableTarget.Process);
                _aggregateLogger.LogInfo($"Process Environment = {EnvironmentVariableTarget.Process}");

                if (await AnythingToProcess())
                {
                    var batchNumber = await _assessorServiceApi.GenerateBatchNumber();
                    var certificates = (await _assessorServiceApi.GetCertificatesToBePrinted()).ToList();

                    await _coverLetterService.Create(batchNumber, certificates);
                    await _ifaCertificateService.Create(batchNumber, certificates);

                    await _notificationService.Send();

                    await _assessorServiceApi.ChangeStatusToPrinted(batchNumber, certificates);
                }
                else
                {
                    _aggregateLogger.LogInfo("Nothing to Process");
                }
            }
            catch (Exception e)
            {
                _aggregateLogger.LogError("Function Errored", e);
                throw;
            }
        }

        private async Task<bool> AnythingToProcess()
        {
            return (await _assessorServiceApi.GetCertificatesToBePrinted()).Any();
        }
    }
}
