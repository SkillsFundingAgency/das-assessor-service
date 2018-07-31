using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.AssessorService.EpaoImporter.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationsApi _notificationsApi;
        private readonly IAggregateLogger _aggregateLogger;
        private readonly IWebConfiguration _webConfiguration;
        private readonly IAssessorServiceApi _assessorServiceApi;

        public NotificationService(INotificationsApi notificationsApi,
            IAggregateLogger aggregateLogger,
            IWebConfiguration webConfiguration,
            IAssessorServiceApi assessorServiceApi)
        {
            _notificationsApi = notificationsApi;
            _aggregateLogger = aggregateLogger;
            _webConfiguration = webConfiguration;
            _assessorServiceApi = assessorServiceApi;
        }

        public async Task Send(int batchNumber, List<CertificateResponse> certificateResponses)
        {
            var emailTemplate = await _assessorServiceApi.GetEmailTemplate();

            var certificatesFileName = $"IFA-Certificate-{GetMonthYear()}-{batchNumber.ToString().PadLeft(3, '0')}.xlsx";

            var personalisation = CreatePersonalisationTokens(certificateResponses, certificatesFileName);

            _aggregateLogger.LogInfo("Send Email");
            _aggregateLogger.LogInfo($"Base Url = {_webConfiguration.NotificationsApiClientConfiguration.ApiBaseUrl}");
            _aggregateLogger.LogInfo($"Client Token = {_webConfiguration.NotificationsApiClientConfiguration.ClientToken}");

            var recipients = emailTemplate.Recipients.Split(';').Select(x => x.Trim());
            foreach (var recipient in recipients)
            {
                var email = new Email
                {
                    RecipientsAddress = recipient,
                    TemplateId = emailTemplate.TemplateId,
                    ReplyToAddress = "jcoxhead@hotmail.com",
                    Subject = "Test Subject",
                    SystemId = "PrintAssessorCoverLetters",
                    Tokens = personalisation
                };

                await _notificationsApi.SendEmail(email);
            }
        }

        private Dictionary<string, string> CreatePersonalisationTokens(List<CertificateResponse> certificateResponses, string certificatesFileName)
        {
            var personalisation = new Dictionary<string, string>
            {
                {"fileName", $"Certificates File Name:- {certificatesFileName}"},
                {
                    "numberOfCertificatesToBePrinted",
                    $"Number Of Certificates to be Printed:- {certificateResponses.Count}"
                },
                {"numberOfCoverLetters", ""},
                {"sftpUploadDirectory", $"{_webConfiguration.Sftp.UploadDirectory}"},
                {"proofDirectory", $"{_webConfiguration.Sftp.ProofDirectory}"}
            };
            return personalisation;
        }

        private static string GetMonthYear()
        {
            var month = DateTime.Today.Month.ToString().PadLeft(2, '0');

            var year = DateTime.Now.Year;
            var monthYear = month + year.ToString().Substring(2, 2);
            return monthYear;
        }
    }
}
