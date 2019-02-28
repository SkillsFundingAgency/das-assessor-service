namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    using System.Collections.Generic;
    using Settings;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public class RoatpApiClient : IRoatpApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<RoatpApiClient> _logger;
        private readonly IRoatpTokenService _tokenService;
        private IWebConfiguration _configuration;
        private string _baseUrl;

        public RoatpApiClient(ILogger<RoatpApiClient> logger, IRoatpTokenService tokenService, IWebConfiguration configuration)
        {
            _logger = logger;
            _tokenService = tokenService;
            _configuration = configuration;
            _baseUrl = _configuration.RoatpApiClientBaseUrl;
            _client = new HttpClient { BaseAddress = new Uri($"{_baseUrl}") };
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetAuditHistory()
        {
            string url = $"{_baseUrl}/api/v1/download/audit";
            _logger.LogInformation($"Retrieving register audit history data from {url}");

            return await Get<IEnumerable<IDictionary<string, object>>>(url);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetCompleteRegister()
        {
            string url = "{_baseUrl}/api/v1/download/complete";
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
