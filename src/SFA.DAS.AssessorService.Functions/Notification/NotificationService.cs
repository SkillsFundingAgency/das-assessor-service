using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Functions.Settings;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.AssessorService.Functions.Notification
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
                RecipientsAddress = "john.coxhead@digital.education.gov.uk",
                TemplateId = _webConfiguration.EmailTemplateSettings.TemplateId,
                ReplyToAddress = "jcoxhead@hotmail.com",
                Subject = "XXXXXX",
                SystemId = "SFA.DAS.AssessorService.PrintFunctionProcessFlow",
                Tokens = personalisation
            };

            await _notificationsApi.SendEmail(email);
        }
    }
}
