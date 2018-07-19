using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client;

namespace SFA.DAS.AssessorService.Application.Api.External.Infrastructure
{
    public class ApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ApiClient> _logger;
        private readonly ITokenService _tokenService;

        public ApiClient(HttpClient client, ILogger<ApiClient> logger, ITokenService tokenService)
        {
            _client = client;
            _logger = logger;
            _tokenService = tokenService;
        }

        private async Task<T> Get<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var res = await _client.GetAsync(new Uri(uri, UriKind.Relative));
            return await res.Content.ReadAsAsync<T>();
        }

        public async Task<List<OrganisationResponse>> GetAllOrganisations()
        {
            return await Get<List<OrganisationResponse>>("/api/v1/organisations/");
        }
    }
}
