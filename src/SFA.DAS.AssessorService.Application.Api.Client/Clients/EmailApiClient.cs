using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class EmailApiClient : ApiClientBase, IEmailApiClient
    {
        private readonly ILogger<EmailApiClient> _logger;

        public EmailApiClient(string baseUri, ITokenService tokenService, ILogger<EmailApiClient> logger) : base(baseUri, tokenService, logger)
        {
            _logger = logger;
        }

        public EmailApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<ApiClientBase> logger) : base(httpClient, tokenService, logger)
        {
        }
        public async Task<EMailTemplate> GetEmailTemplate(string templateName)
        {

            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/emailTemplates/{templateName}"))
            {
                return await RequestAndDeserialiseAsync<EMailTemplate>(request, $"Could not find the template");
            }
            
        }

        public async Task SendEmailWithTemplate(SendEmailRequest sendEmailRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/emailTemplates/"))
            {
                _logger.LogInformation("Sending Email");
                 await PostPutRequest(request,sendEmailRequest);
            }
        }
    }
}
