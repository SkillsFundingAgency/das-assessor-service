using System;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.DomainServices;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.EMail;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Logger;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow
{
    public class Command
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly CoverLetterService _coverLetterService;
        private readonly IFACertificateService _ifaCertificateService;
        private readonly EMailSender _emailSender;

        public Command(IAggregateLogger aggregateLogger,
            CoverLetterService coverLetterService,
            IFACertificateService ifaCertificateService,
            EMailSender emailSender)
        {
            _aggregateLogger = aggregateLogger;
            _coverLetterService = coverLetterService;
            _ifaCertificateService = ifaCertificateService;
            _emailSender = emailSender;
        }

        public async Task Execute()
        {
            _aggregateLogger.LogInfo("Print Function Flow Started");

            await _emailSender.SendEMail();

            //_aggregateLogger.LogInfo("101 Azure Function Demo - Accessing Environment variables");
            //var customSetting = Environment.GetEnvironmentVariable("CustomSetting", EnvironmentVariableTarget.Process);
            //_aggregateLogger.LogInfo($"Process Environment = {EnvironmentVariableTarget.Process}");

            //await _coverLetterService.Create();
            //await _ifaCertificateService.Create();
        }
    }
}
