using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public class DfeSignInService: IDfeSignInService
    {
        private readonly IWebConfiguration _config;
        private readonly ILogger<DfeSignInService> _logger;

        public DfeSignInService(IWebConfiguration config, ILogger<DfeSignInService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<InviteUserResponse> InviteUser(string email, string givenName, string familyName, Guid userId)
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(_config.DfeSignIn.MetadataAddress);
            if (disco.IsError)
            {
                _logger.LogError(disco.Error);
            }

            // request token
            var tokenResponse = client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "client",
                ClientSecret = _config.DfeSignIn.ApiClientSecret,
                Scope = "api1"
            }).Result;

            if (tokenResponse.IsError)
                _logger.LogError(tokenResponse.Error);

            _logger.LogInformation(tokenResponse.Json.ToString());

            using (var httpClient = new HttpClient())
            {
                httpClient.SetBearerToken(tokenResponse.AccessToken);
                var inviteJson = JsonConvert.SerializeObject(new
                {
                    sourceId = userId.ToString(),
                    given_name = givenName,
                    family_name = familyName,
                    email,
                    userRedirect = _config.DfeSignIn.RedirectUri,
                    callback = _config.DfeSignIn.CallbackUri
                });

                var response = await httpClient.PostAsync(_config.DfeSignIn.ApiUri,
                    new StringContent(inviteJson, Encoding.UTF8, "application/json")
                );

                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Returned from DfE Invitation Service. Status Code: {0}. Message: {0}",
                    (int)response.StatusCode, content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error from DfE Invitation Service. Status Code: {0}. Message: {0}",
                        (int)response.StatusCode, content);
                    return new InviteUserResponse() { IsSuccess = false };
                }

                return new InviteUserResponse();
            }
        }
    }
}