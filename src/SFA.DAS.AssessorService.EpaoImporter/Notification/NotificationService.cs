using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task Send()
        {
            var emailTemplate = await _assessorServiceApi.GetEmailTemplate();

            var personalisation = new Dictionary<string, string>
                {{"name", "john coxhead"}, {"day ", "1"}, {"day of week", "Monday"}, {"colour", "blue"}};

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
    }
}
