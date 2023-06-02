﻿using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class LoginApiClient : ApiClientBase, ILoginApiClient
    {
        public LoginApiClient(HttpClient httpClient, IAssessorTokenService tokenService, ILogger<ApiClientBase> logger)
            : base(httpClient, tokenService, logger)
        {
        }

        public async Task<LoginResponse> Login(LoginRequest searchQuery)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/login"))
            {
                return await PostPutRequestWithResponse<LoginRequest, LoginResponse>(request, searchQuery);
            }
        }
    }
}