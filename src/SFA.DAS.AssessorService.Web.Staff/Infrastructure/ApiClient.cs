using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;

namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    public class ApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ApiClient> _logger;

        public ApiClient(HttpClient client, ILogger<ApiClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<List<CertificateResponse>> GetCertificates()
        {
            try
            {
                var res = await _client.GetAsync(new Uri("/api/v1/certificates?statusses=Submitted", UriKind.Relative));
                res.EnsureSuccessStatusCode();
                return await res.Content.ReadAsAsync<List<CertificateResponse>>();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Error connecting to API: {e}");
                throw;
            }
        }
    }
}