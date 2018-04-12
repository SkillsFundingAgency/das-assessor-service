using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Data;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.DomainServices;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Logger;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Notification;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow
{
    public class Command
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly CoverLetterService _coverLetterService;
        private readonly IFACertificateService _ifaCertificateService;
        private readonly ICertificatesRepository _certificatesRepository;
        private readonly NotificationService _notificationService;

        public Command(IAggregateLogger aggregateLogger,
            CoverLetterService coverLetterService,
            IFACertificateService ifaCertificateService,
            ICertificatesRepository certificatesRepository,
            NotificationService notificationService)
        {
            _aggregateLogger = aggregateLogger;
            _coverLetterService = coverLetterService;
            _ifaCertificateService = ifaCertificateService;
            _certificatesRepository = certificatesRepository;
            _notificationService = notificationService;
        }

        public async Task Execute()
        {
            _aggregateLogger.LogInfo("Print Function Flow Started");

            _aggregateLogger.LogInfo("101 Azure Function Demo - Accessing Environment variables");
            var customSetting =
                Environment.GetEnvironmentVariable("CustomSetting", EnvironmentVariableTarget.Process);
            _aggregateLogger.LogInfo($"Process Environment = {EnvironmentVariableTarget.Process}");

            if (await AnythingToProcess())
            {
                var batchNumber = await _certificatesRepository.GenerateBatchNumber();
                var certificates = (await _certificatesRepository.GetCertificatesToBePrinted()).ToList();

                await _coverLetterService.Create(batchNumber, certificates);
                await _ifaCertificateService.Create(batchNumber, certificates);

                await _notificationService.Send();
            }
            else
            {
                _aggregateLogger.LogInfo("Nothing to Process");
            }
        }

        private async Task<bool> AnythingToProcess()
        {
            return (await _certificatesRepository.GetCertificatesToBePrinted()).Any();
        }
    }
}
