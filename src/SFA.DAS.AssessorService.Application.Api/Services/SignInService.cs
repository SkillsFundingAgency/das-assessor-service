﻿using System;
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
    public class SignInService: ISignInService
    {
        private readonly IApiConfiguration _config;
        private readonly ILogger<SignInService> _logger;

        public SignInService(IApiConfiguration config, ILogger<SignInService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<InviteUserResponse> InviteUser(string email, string givenName, string familyName, Guid userId)
        {
            var tokenResponse = await GetAuthorizationToken();

            using (var httpClient = new HttpClient())
            {
                httpClient.SetBearerToken(tokenResponse.AccessToken);
                var inviteJson = JsonConvert.SerializeObject(new
                {
                    sourceId = userId.ToString(),
                    given_name = givenName,
                    family_name = familyName,
                    email,
                    userRedirect = _config.LoginService.RedirectUri,
                    callback = _config.LoginService.CallbackUri
                });

                var response = await httpClient.PostAsync(_config.LoginService.ApiUri,
                    new StringContent(inviteJson, Encoding.UTF8, "application/json")
                );

                var content = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<CreateInvitationResponse>(content);

                _logger.LogInformation("Returned from DfE Invitation Service. Status Code: {0}. Message: {0}",
                    (int) response.StatusCode, content);

                if (response.IsSuccessStatusCode)
                    return responseObject.Message == "User already exists"
                        ? new InviteUserResponse() {UserExists = true, IsSuccess = false, ExistingUserId = responseObject.ExistingUserId}
                        : new InviteUserResponse();
                
                _logger.LogError("Error from DfE Invitation Service. Status Code: {0}. Message: {0}",
                    (int) response.StatusCode, content);
                return new InviteUserResponse() {IsSuccess = false};
            }
        }
        
        public async Task<InviteUserResponse> InviteUserToOrganisation(string email, string givenName, string familyName, Guid userId, string organisationName, string inviter)
        {
            var tokenResponse = await GetAuthorizationToken();
            
            using (var httpClient = new HttpClient())
            {
                httpClient.SetBearerToken(tokenResponse.AccessToken);
                var inviteJson = JsonConvert.SerializeObject(new
                {
                    sourceId = userId.ToString(),
                    given_name = givenName,
                    family_name = familyName,
                    email,
                    userRedirect = _config.LoginService.RedirectUri,
                    callback = _config.LoginService.CallbackUri, 
                    organisationName,
                    inviter
                });

                var inviteUrl = _config.LoginService.ApiUri + "/inviteToOrganisation";
                
                var response = await httpClient.PostAsync(inviteUrl,
                    new StringContent(inviteJson, Encoding.UTF8, "application/json")
                );

                var content = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<CreateInvitationResponse>(content);

                _logger.LogInformation("Returned from DfE Invitation Service. Status Code: {0}. Message: {0}",
                    (int) response.StatusCode, content);

                if (response.IsSuccessStatusCode)
                    return responseObject.Message == "User already exists"
                        ? new InviteUserResponse() {UserExists = true, IsSuccess = false, ExistingUserId = responseObject.ExistingUserId}
                        : new InviteUserResponse();
                
                _logger.LogError("Error from DfE Invitation Service. Status Code: {0}. Message: {0}",
                    (int) response.StatusCode, content);
                return new InviteUserResponse() {IsSuccess = false};
            }
        }

        private async Task<TokenResponse> GetAuthorizationToken()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(_config.LoginService.MetadataAddress);
            if (disco.IsError)
            {
                _logger.LogError(disco.Error);
            }

            // request token
            var tokenResponse = client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "client",
                ClientSecret = _config.LoginService.ApiClientSecret,
                Scope = "api1"
            }).Result;

            if (tokenResponse.IsError)
                _logger.LogError(tokenResponse.Error);

            _logger.LogInformation(tokenResponse.Json.ToString());
            return tokenResponse;
        }

        

        private class CreateInvitationResponse
        {
            public string Message { get; set; }
            public bool Invited { get; set; }
            public Guid InvitationId { get; set; }
            public Guid? ExistingUserId { get; set; }
        }
    }
}