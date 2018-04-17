using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.EpaoImporter.Settings;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.AssessorService.EpaoImporter.Notification
{
    public class NotificationService
    {
        private readonly INotificationsApi _notificationsApi;
        private readonly IWebConfiguration _webConfiguration;

        public NotificationService(INotificationsApi notificationsApi,
            IWebConfiguration webConfiguration)
        {
            _notificationsApi = notificationsApi;
            _webConfiguration = webConfiguration;
        }

        public async Task Send()
        {
            var personalisation = new Dictionary<string, string>
                {{"name", "john coxhead"}, {"day ", "1"}, {"day of week", "Monday"}, {"colour", "blue"}};

            
             var email = new Email
            {
                //RecipientsAddress = _webConfiguration.EmailTemplateSettings.RecipientsAddress,
                RecipientsAddress = "jcoxhead@hotmail.com",
                TemplateId = _webConfiguration.EmailTemplateSettings.TemplateId,
                ReplyToAddress = _webConfiguration.EmailTemplateSettings.ReplyToAddress,
                Subject = _webConfiguration.EmailTemplateSettings.Subject,
                SystemId = _webConfiguration.EmailTemplateSettings.SystemId,
                Tokens = personalisation
            };

            await _notificationsApi.SendEmail(email);
        }
    }
}
