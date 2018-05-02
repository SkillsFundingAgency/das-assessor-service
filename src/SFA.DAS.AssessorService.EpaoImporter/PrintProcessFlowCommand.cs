using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.Interfaces;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.Notification;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public class PrintProcessFlowCommand : ICommand
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly ICoverLetterService _coverLetterService;
        private readonly IIFACertificateService _ifaCertificateService;
        private readonly IAssessorServiceApi _assessorServiceApi;
        private readonly INotificationService _notificationService;

        public PrintProcessFlowCommand(IAggregateLogger aggregateLogger,
            ICoverLetterService coverLetterService,
            IIFACertificateService ifaCertificateService,
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
                _aggregateLogger.LogInfo($"Process Environment = {EnvironmentVariableTarget.Process}");

                if (await AnythingToProcess())
                {
                    var batchNumber = await _assessorServiceApi.GenerateBatchNumber();
                    var certificates = (await _assessorServiceApi.GetCertificatesToBePrinted()).ToList();

                    var coverLettersProduced = await _coverLetterService.Create(batchNumber, certificates);
                    await _ifaCertificateService.Create(batchNumber, certificates, coverLettersProduced);

                    await _notificationService.Send(batchNumber, certificates, coverLettersProduced);

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
