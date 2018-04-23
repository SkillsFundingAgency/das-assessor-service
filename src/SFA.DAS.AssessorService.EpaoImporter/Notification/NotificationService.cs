using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.AssessorService.EpaoImporter.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationsApi _notificationsApi;
        private readonly IAssessorServiceApi _assessorServiceApi;
        private readonly IWebConfiguration _webConfiguration;

        public NotificationService(INotificationsApi notificationsApi,
            IAssessorServiceApi assessorServiceApi,
            IWebConfiguration webConfiguration)
        {
            _notificationsApi = notificationsApi;
            _assessorServiceApi = assessorServiceApi;
            _webConfiguration = webConfiguration;
        }

        public async Task Send(int batchNumber, List<CertificateResponse> certificates,
            int coverLettersProduced)
        {
            var emailTemplate = await _assessorServiceApi.GetEmailTemplate();

            var fileName = $"IFA-Certificate-{GetMonthYear()}-{batchNumber.ToString().PadLeft(3, '0')}.xlsx";

            var personalisation = new Dictionary<string, string>
                {{"fileName", $"File Name:- {fileName}" },
                    { "numberOfCertificatesToBePrinted", $"Number Of Certificates to be Printed:- {certificates.Count}"},
                    { "numberOfCoverLetters", $"Number of Cover Letters:- {coverLettersProduced}"},
                    { "epaos", "John\r\nJames\r\nJane\r\n" }};

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

        private static string GetMonthYear()
        {
            var month = DateTime.Today.Month.ToString().PadLeft(2, '0');

            var year = DateTime.Now.Year;
            var monthYear = month + year.ToString().Substring(2, 2);
            return monthYear;
        }
    }
}
