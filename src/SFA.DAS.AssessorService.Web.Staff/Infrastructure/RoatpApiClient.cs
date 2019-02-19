namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    using System.Collections.Generic;
    using Settings;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.AssessorService.Application.Api.Client;
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public class RoatpApiClient : IRoatpApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<RoatpApiClient> _logger;
        private readonly ITokenService _tokenService;
        private IWebConfiguration _configuration;
        private string _baseUrl;

        public RoatpApiClient(ILogger<RoatpApiClient> logger, ITokenService tokenService, IWebConfiguration configuration)
        {
            _logger = logger;
            _tokenService = tokenService;
            _configuration = configuration;
            _baseUrl = _configuration.RoatpApiClientBaseUrl;
            _client = new HttpClient { BaseAddress = new Uri($"{_baseUrl}") };
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetAuditHistory()
        {
            return await Get<IEnumerable<IDictionary<string, object>>>($"{_baseUrl}/api/v1/download/audit");
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetCompleteRegister()
        {
            return await Get<IEnumerable<IDictionary<string, object>>>($"{_baseUrl}/api/v1/download/complete");
        }

        private async Task<T> Get<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            using (var response = await _client.GetAsync(new Uri(uri, UriKind.Absolute)))
            {
                return await response.Content.ReadAsAsync<T>();
            }
        }

    }
}
