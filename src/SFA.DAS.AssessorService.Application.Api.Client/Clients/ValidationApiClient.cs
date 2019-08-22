using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class ValidationApiClient : ApiClientBase, IValidationApiClient
    {
        private readonly ILogger<ValidationApiClient> _logger;

        public ValidationApiClient(string baseUri, ITokenService tokenService, ILogger<ValidationApiClient> logger) : base(baseUri, tokenService, logger)
        {
            _logger = logger;
        }

        public ValidationApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<ApiClientBase> logger) : base(httpClient, tokenService, logger)
        {
        }

        public async Task<bool> ValidatePhoneNumber(string phoneNumberToValidate)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/validation/is-phonenumber-format/{phoneNumberToValidate}"))
            {
                return await RequestAndDeserialiseAsync<bool>(request, $"Could not validate the phone number");
            }
        }

        public async Task<bool> ValidateEmailAddress(string emailToValidate)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/validation/is-email-format/{emailToValidate}"))
            {
                return await RequestAndDeserialiseAsync<bool>(request, $"Could not validate the email address");
            }
        }

        public async Task<bool> ValidateWebsiteLink(string websiteLinkToValidate)
        {
            var url = $"/api/validation/is-websitelink-format/{websiteLinkToValidate}";
            _logger.LogInformation($"VALIDATEWEBSITELINK - ValidationApiClient.ValidateWebsiteLink API url called: {url}");
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                return await RequestAndDeserialiseAsync<bool>(request, $"Could not validate the website address");
            }
        }
    }
}
