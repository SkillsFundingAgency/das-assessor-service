using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Extensions;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.InfrastructureServices;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Logger;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Settings;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.Data
{
    public class CertificatesRepository : ICertificatesRepository
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly HttpClient _httpClient;
        private readonly IWebConfiguration _webConfiguration;
        private readonly TokenService _tokenService;

        public CertificatesRepository(IAggregateLogger aggregateLogger,
            HttpClient httpClient,
            IWebConfiguration webConfiguration,
            TokenService tokenService)
        {
            _aggregateLogger = aggregateLogger;
            _httpClient = httpClient;
            _webConfiguration = webConfiguration;
            _tokenService = tokenService;
        }

        public async Task<IEnumerable<CertificateResponse>> GetCertificatesToBePrinted()
        {
            var response = await _httpClient.GetAsync(
                "/api/v1/certificates");

            var certificates = response.Deserialise<List<CertificateResponse>>();
            if (response.IsSuccessStatusCode)
            {
                _aggregateLogger.LogInfo($"Status code returned: {response.StatusCode}. Content: {response.Content.ReadAsStringAsync().Result}");
            }
            else
            {
                _aggregateLogger.LogInfo($"Status code returned: {response.StatusCode}. Content: {response.Content.ReadAsStringAsync().Result}");
            }

            return certificates;
        }

        public async Task<int> GenerateBatchNumber()
        {
            var response = await _httpClient.GetAsync(
                "/api/v1/certificates/generatebatchnumber");

            return response.Deserialise<int>();
        }
    }
}
