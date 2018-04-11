using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Data;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.DomainServices;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.EMail;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Logger;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Notification;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow
{
    public class Command
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly CoverLetterService _coverLetterService;
        private readonly IFACertificateService _ifaCertificateService;
        private readonly CertificatesRepository _certificatesRepository;
        private readonly NotificationService _notificationService;
        private readonly EMailSender _emailSender;

        public Command(IAggregateLogger aggregateLogger,
            CoverLetterService coverLetterService,
            IFACertificateService ifaCertificateService,
            CertificatesRepository certificatesRepository,
            NotificationService notificationService,
            EMailSender emailSender)
        {
            _aggregateLogger = aggregateLogger;
            _coverLetterService = coverLetterService;
            _ifaCertificateService = ifaCertificateService;
            _certificatesRepository = certificatesRepository;
            _notificationService = notificationService;
            _emailSender = emailSender;
        }

        public async Task Execute()
        {
            _aggregateLogger.LogInfo("Print Function Flow Started");

            _aggregateLogger.LogInfo("101 Azure Function Demo - Accessing Environment variables");
            var customSetting =
                Environment.GetEnvironmentVariable("CustomSetting", EnvironmentVariableTarget.Process);
            _aggregateLogger.LogInfo($"Process Environment = {EnvironmentVariableTarget.Process}");

            //_notificationService.Send();

            //if (AnythingToProcess())
            //{
            //    //await _emailSender.SendEMail();
            //    await _coverLetterService.Create();
            //    await _ifaCertificateService.Create();
            //}
            //else
            //{
            //    _aggregateLogger.LogInfo("Nothing to Process");
            //}
        }

        private bool AnythingToProcess()
        {
            return _certificatesRepository.GetData().Any();
        }
    }
}
