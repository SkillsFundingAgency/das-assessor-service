using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
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
        private readonly IEMailTemplateQueryRepository _eMailTemplateQueryRepository;
        private readonly ILogger<SendEmailHandler> _logger;

        public SendEmailHandler(INotificationsApi notificationsApi, IEMailTemplateQueryRepository eMailTemplateQueryRepository, ILogger<SendEmailHandler> logger)
        {
            _notificationsApi = notificationsApi;
            _eMailTemplateQueryRepository = eMailTemplateQueryRepository;
            _logger = logger;
        }

      
        public async Task<Unit> Handle(SendEmailRequest message, CancellationToken cancellationToken)
        {
            var emailTemplate = message.EmailTemplateSummary;

            if (emailTemplate != null && !string.IsNullOrWhiteSpace(message.Email))
            {
                var personalisationTokens = GetPersonalisationTokens(message.Tokens);
                await SendEmailViaNotificationsApi(message.Email, emailTemplate.TemplateId, emailTemplate.TemplateName, personalisationTokens);

                //if(emailTemplate.RecipientTemplate != null)
                //    await SendEmailToRecipients(emailTemplate.RecipientTemplate, personalisationTokens);

            }
            else if (emailTemplate is null)
            {
                _logger.LogError($"Cannot find email template ");
            }
            else
            {
                _logger.LogError($"Cannot send email template {emailTemplate.TemplateName} to '{message.Email}'");
            };
            return Unit.Value;
        }

        //private async Task SendEmailToRecipients(string templateName, dynamic tokens)
        //{
        //    var emailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(templateName);

        //    if (emailTemplate != null && emailTemplate.Recipients != null)
        //    {
        //        var personalisationTokens = GetPersonalisationTokens(tokens);

        //        var recipients = emailTemplate.Recipients.Split(';').Select(x => x.Trim());
        //        foreach (var recipient in recipients)
        //        {
        //            await SendEmailViaNotificationsApi(recipient, emailTemplate.TemplateId, emailTemplate.TemplateName, personalisationTokens);
        //        }

        //        await SendEmailToRecipients(emailTemplate.RecipientTemplate, tokens);
        //    }
        //}


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
                    _logger.LogError(je,$"Failed to read personalisation tokens");
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
