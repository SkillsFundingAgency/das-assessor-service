﻿using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class ValidationApiClient : ApiClientBase, IValidationApiClient
    {
        private readonly ILogger<ValidationApiClient> _logger;

        public ValidationApiClient(IAssessorApiClientFactory clientFactory, ILogger<ValidationApiClient> logger)
            : base(clientFactory.CreateHttpClient(), logger)
        {
            _logger = logger;
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
            var url = $"/api/validation/is-websitelink-format?websiteLinkToValidate={websiteLinkToValidate}";
            _logger.LogInformation($"VALIDATEWEBSITELINK - ValidationApiClient.ValidateWebsiteLink API url called: {url}");
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                return await RequestAndDeserialiseAsync<bool>(request, $"Could not validate the website address");
            }
        }
    }
}
