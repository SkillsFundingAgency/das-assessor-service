﻿using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.DTOs;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class EmailApiClient : ApiClientBase, IEmailApiClient
    {
        private readonly ILogger<EmailApiClient> _logger;

        public EmailApiClient(IAssessorApiClientFactory clientFactory, ILogger<EmailApiClient> logger)
            : base(clientFactory.CreateHttpClient(), logger)
        {
            _logger = logger;
        }

        public async Task<EmailTemplateSummary> GetEmailTemplate(string templateName)
        {

            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/emailTemplates/{templateName}"))
            {
                return await RequestAndDeserialiseAsync<EmailTemplateSummary>(request, $"Could not find the template");
            }
            
        }

        public async Task SendEmailWithTemplate(SendEmailRequest sendEmailRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/emailTemplates/"))
            {
                _logger.LogInformation("Sending Email");
                 await PostPutRequestAsync(request,sendEmailRequest);
            }
        }
    }
}
