using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.EpaoImporter.Const;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.AssessorService.EpaoImporter.Notification
{
    public class PrivatelyFundedCertificatesApprovalNotification : IPrivatelyFundedCertificatesApprovalNotification
    {
        private readonly INotificationsApi _notificationsApi;
        private readonly IAggregateLogger _aggregateLogger;
        private readonly IWebConfiguration _webConfiguration;
        private readonly IAssessorServiceApi _assessorServiceApi;

        public PrivatelyFundedCertificatesApprovalNotification(INotificationsApi notificationsApi,
            IAggregateLogger aggregateLogger,
            IWebConfiguration webConfiguration,
            IAssessorServiceApi assessorServiceApi)
        {
            _notificationsApi = notificationsApi;
            _aggregateLogger = aggregateLogger;
            _webConfiguration = webConfiguration;
            _assessorServiceApi = assessorServiceApi;
        }

        public async Task Send(IEnumerable<CertificateResponse> certificateResponses)
        {
            var emailTemplate = await _assessorServiceApi.GetEmailTemplate(EMailTemplateNames.PrivatelyFundedCertificatesApprovals);
            

            var personalisation = CreatePersonalisationTokens(certificateResponses);

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
                    SystemId = "PrivatelyFundedCertificatesApprovals",
                    Tokens = personalisation
                };

                await _notificationsApi.SendEmail(email);
            }
        }

        private Dictionary<string, string> CreatePersonalisationTokens(IEnumerable<CertificateResponse> certificateResponses)
        {
            var certificateReferences = certificateResponses.Select(q => q.CertificateReference).ToList();
            var stringifiedCoverLetterFileNames = GetCoverLetterFileNames(certificateReferences);            

            var personalisation = new Dictionary<string, string>
            {              
                {
                   "numberOfCertificatesToBeApproved",
                    $"Number Of Certificates to be Approved:- {certificateReferences.Count}"
                },
                {"certificateReferences", $"{stringifiedCoverLetterFileNames}"}              
            };
            return personalisation;
        }

        private static StringBuilder GetCoverLetterFileNames(List<string> certificateReferences)
        {
            var stringifiedFileName = new StringBuilder();
            foreach (var certificateReference in certificateReferences)
            {
                stringifiedFileName.Append(certificateReference);
                if (certificateReference != certificateReferences.Last())
                    stringifiedFileName.Append("\r\n");
            }

            return stringifiedFileName;
        }       
    }
}
