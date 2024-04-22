using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NServiceBus;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.DTOs;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.Notifications.Messages.Commands;

namespace SFA.DAS.AssessorService.Application.Handlers.EmailHandlers
{
    public class SendEmailHandler : IRequestHandler<SendEmailRequest>
    {
        private const string SystemId = "AssessorService";
        private const string ReplyToAddress = "digital.apprenticeship.service@notifications.service.gov.uk";
        private const string Subject = "EPAO user to approve";      
        private readonly ILogger<SendEmailHandler> _logger;
        private readonly IMessageSession _messageSession;

        public SendEmailHandler(ILogger<SendEmailHandler> logger, IMessageSession messageSession)
        {       
            _logger = logger;
            _messageSession = messageSession;
        }

      
        public async Task<Unit> Handle(SendEmailRequest message, CancellationToken cancellationToken)
        {
            var emailTemplateSummary = message.EmailTemplateSummary;            
            var personalisationTokens = GetPersonalisationTokens(message.Tokens);

            if (emailTemplateSummary != null && !string.IsNullOrWhiteSpace(message.Email))
            {                
                await SendEmailViaNserviceBus(message.Email, emailTemplateSummary.TemplateId, emailTemplateSummary.TemplateName, personalisationTokens);
            }
            else if (emailTemplateSummary != null && emailTemplateSummary.Recipients != string.Empty)
            {
                await SendEmailToTemplateRecipients(emailTemplateSummary, personalisationTokens);
            }
            else if (emailTemplateSummary is null)
            {
                _logger.LogError($"Cannot find email template ");
            }
            else
            {
                _logger.LogError($"Cannot send email template {emailTemplateSummary.TemplateName} to '{message.Email}'");
            };
            return Unit.Value;
        }      

        private Dictionary<string, string> GetPersonalisationTokens(dynamic tokens)
        {
            var personalisationTokens = new Dictionary<string, string>();

            if (tokens != null)
            {
                try
                {
                    personalisationTokens = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(tokens));
                }
                catch (JsonException je)
                {
                    _logger.LogError(je,$"Failed to read personalisation tokens");
                }
            }

            return personalisationTokens;
        }

        private async Task SendEmailToTemplateRecipients(EmailTemplateSummary emailTemplateSummary, dynamic personalisationTokens)
        {
            var recipients = emailTemplateSummary.Recipients.Split(';').Select(x => x.Trim());
            foreach (var recipient in recipients)
            {
                await SendEmailViaNserviceBus(recipient, emailTemplateSummary.TemplateId, emailTemplateSummary.TemplateName, personalisationTokens);
            }
        }


        private async Task SendEmailViaNserviceBus(string toAddress, string templateId, string templateName, Dictionary<string, string> personalisationTokens)
        {
            try
            {
                var emailCommand = new SendEmailCommand(templateId, toAddress, personalisationTokens);
                _logger.LogInformation($"Sending {templateName} email ({templateId}) to {toAddress}");
                await _messageSession.Send(emailCommand);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending {templateName} email ({templateId}) to {toAddress}");
            }
        }


    }
}
