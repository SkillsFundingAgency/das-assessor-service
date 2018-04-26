using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task Send(int batchNumber, List<CertificateResponse> certificateResponses,
            List<string> coverLetterFileNames)
        {
            var emailTemplate = await _assessorServiceApi.GetEmailTemplate();

            var certificatesFileName = $"IFA-Certificate-{GetMonthYear()}-{batchNumber.ToString().PadLeft(3, '0')}.xlsx";

            var strinfigiedEndPointAssessorOrganisations = GetStringifiedEndPointOrganisations(certificateResponses);
            var stringifiedCoverLetterFileNames = GetStringifiedCoverLetterFileNames(coverLetterFileNames);

            var personalisation = CreatePersonalisationTokens(certificateResponses, coverLetterFileNames, certificatesFileName, strinfigiedEndPointAssessorOrganisations, stringifiedCoverLetterFileNames);

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

        private static Dictionary<string, string> CreatePersonalisationTokens(List<CertificateResponse> certificateResponses,
            List<string> coverLetterFileNames, string certificatesFileName,
            string strinfigiedEndPointAssessorOrganisations, StringBuilder stringifiedCoverLetterFileNames)
        {
            var personalisation = new Dictionary<string, string>
            {
                {"fileName", $"Certificates File Name:- {certificatesFileName}"},
                {
                    "numberOfCertificatesToBePrinted",
                    $"Number Of Certificates to be Printed:- {certificateResponses.Count}"
                },
                {"numberOfCoverLetters", $"Number of Cover Letters:- {coverLetterFileNames.Count}"},
                {"epaos", $"{strinfigiedEndPointAssessorOrganisations}"},
                {"coverLetterFileNames", $"{stringifiedCoverLetterFileNames}"}
            };
            return personalisation;
        }

        private static StringBuilder GetStringifiedCoverLetterFileNames(List<string> coverLetterFileNames)
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

        private static string GetStringifiedEndPointOrganisations(List<CertificateResponse> certificateResponses)
        {
            var endPointAssessorOrganisations = certificateResponses.ToArray().GroupBy(
                    x => new
                    {
                        x.EndPointAssessorOrganisationName
                    },
                    (key, group) => new
                    {
                        key1 = key.EndPointAssessorOrganisationName,
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
