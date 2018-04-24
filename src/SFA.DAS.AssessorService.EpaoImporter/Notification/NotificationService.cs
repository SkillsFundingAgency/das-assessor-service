using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task Send(int batchNumber, List<CertificateResponse> certificateResponses,
            List<string> coverLetterFileNames)
        {
            var emailTemplate = await _assessorServiceApi.GetEmailTemplate();
            var fileName = $"IFA-Certificate-{GetMonthYear()}-{batchNumber.ToString().PadLeft(3, '0')}.xlsx";

            var endPointAssessorOrganisations = GetEndPointOrganisations(certificateResponses);

            var stringifiedCoverLetterFileNames = GetCoverLetterFileNames(coverLetterFileNames);

            var personalisation = new Dictionary<string, string>
            {
                {"fileName", $"Certificates File Name:- {fileName}"},
                {
                    "numberOfCertificatesToBePrinted",
                    $"Number Of Certificates to be Printed:- {certificateResponses.Count}"
                },
                {"numberOfCoverLetters", $"Number of Cover Letters:- {coverLetterFileNames.Count}"},
                {"epaos", $"{endPointAssessorOrganisations}"},
                {"coverLetterFileNames", $"{stringifiedCoverLetterFileNames}"}
            };

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

        private static StringBuilder GetCoverLetterFileNames(List<string> coverLetterFileNames)
        {
            var stringifiedFileName = new StringBuilder();
            foreach (var coverLetterFileName in coverLetterFileNames)
            {
                stringifiedFileName.Append(coverLetterFileName);
                if(coverLetterFileName != coverLetterFileNames.Last())
                stringifiedFileName.Append("\r\n");
            }

            return stringifiedFileName;
        }

        private static string GetEndPointOrganisations(List<CertificateResponse> certificateResponses)
        {
            var endPointAssessorOrganisations = certificateResponses.ToArray().GroupBy(
                    x => new
                    {
                        x.CertificateData.ContactOrganisation,
                    },
                    (key, group) => new
                    {
                        key1 = key.ContactOrganisation,
                        Result = @group.ToList()
                    }).Select(x => x.key1).ToList()
                .Distinct();

            var organisations = new StringBuilder();
            foreach (var organisation in endPointAssessorOrganisations)
            {
                organisations.Append(organisation);
                organisations.Append("\r\n");
            }

            return organisations.ToString();
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
