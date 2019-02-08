using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.AssessorService.Application.Handlers.EmailHandlers
{
    public class SendEmailHandler : IRequestHandler<SendEmailRequest>
    {
        private const string SystemId = "AssessorService";
        private const string ReplyToAddress = "digital.apprenticeship.service@notifications.service.gov.uk";
        private const string Subject = "EPAO user to approve";
        private readonly INotificationsApi _notificationsApi;
        private readonly ILogger<SendEmailHandler> _logger;

        public SendEmailHandler(INotificationsApi notificationsApi, ILogger<SendEmailHandler> logger)
        {
            _notificationsApi = notificationsApi;
            _logger = logger;
        }

      
        public async Task Handle(SendEmailRequest message, CancellationToken cancellationToken)
        {
            var emailTemplate = message.EmailTemplate;

            if (emailTemplate != null && !string.IsNullOrWhiteSpace(message.Email))
            {
                var personalisationTokens = GetPersonalisationTokens(message.Tokens);
                await SendEmailViaNotificationsApi(message.Email, emailTemplate.TemplateId, emailTemplate.TemplateName, personalisationTokens);

            }
            else if (emailTemplate is null)
            {
                _logger.LogError($"Cannot find email template ");
            }
            else
            {
                _logger.LogError($"Cannot send email template {emailTemplate.TemplateName} to '{message.Email}'");
            };
        }

        private Dictionary<string, string> GetPersonalisationTokens(dynamic tokens)
        {
            var personalisationTokens = new Dictionary<string, string>();

            if (tokens != null)
            {
                try
                {

                    personalisationTokens =
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(tokens));
                }
                catch (JsonException je)
                {
                    _logger.LogError($"Failed to read personalisation tokens : {je.Message}");
                }
            }

            return personalisationTokens;
        }

        private async Task SendEmailViaNotificationsApi(string toAddress, string templateId, string templateName, Dictionary<string, string> personalisationTokens)
        {
            // Note: It appears that if anything is hard copied in the template it'll ignore any values below
            var email = new Email
            {
                RecipientsAddress = toAddress,
                TemplateId = templateId,
                ReplyToAddress = ReplyToAddress,
                Subject = Subject,
                SystemId = SystemId,
                Tokens = personalisationTokens
            };

            try
            {
                _logger.LogInformation($"Sending {templateName} email ({templateId}) to {toAddress}");
                await Task.Run(() => _notificationsApi.SendEmail(email));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending {templateName} email ({templateId}) to {toAddress}");
            }
        }

        
    }
}
