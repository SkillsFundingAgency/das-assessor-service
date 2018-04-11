using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Client.Configuration;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.Notification
{
    public class NotificationService
    {
        private readonly INotificationsApi _notificationsApi;
        private readonly INotificationsApiClientConfiguration _notificationsApiClientConfiguration;

        public NotificationService(INotificationsApi notificationsApi, INotificationsApiClientConfiguration notificationsApiClientConfiguration)
        {
            _notificationsApi = notificationsApi;
            _notificationsApiClientConfiguration = notificationsApiClientConfiguration;
        }

        public async Task Send()
        {
            var personalisation = new Dictionary<string, string>
                {{"name", "john coxhead"}, {"day ", "1"}, {"day of week", "Monday"}, {"colour", "blue"}};

            Email email = new Email
            {
                RecipientsAddress = "john.coxhead@digital.education.gov.uk",
                TemplateId = "5b171b91-d406-402a-a651-081cce820acb",
                ReplyToAddress = "jcoxhead@hotmail.com",
                Subject = "XXXXXX",
                SystemId = "SFA.DAS.AssessorService.PrintFunctionProcessFlow",
                Tokens = personalisation
            };

            await _notificationsApi.SendEmail(email);
        }
    }
}
