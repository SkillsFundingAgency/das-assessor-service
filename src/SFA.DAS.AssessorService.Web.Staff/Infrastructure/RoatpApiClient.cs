namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using Settings;
    using Microsoft.Extensions.Logging;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using SFA.DAS.AssessorService.Api.Types.Models.Roatp;

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
            _logger.LogInformation($"Retrieving RoATP register audit history data from {url}");

            return await Get<IEnumerable<IDictionary<string, object>>>(url);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetCompleteRegister()
        {
            string url = "{_baseUrl}/api/v1/download/complete";
            _logger.LogInformation($"Retrieving RoATP complete register data from {url}");
            return await Get<IEnumerable<IDictionary<string, object>>>($"{_baseUrl}/api/v1/download/complete");
        }

        public async Task<IEnumerable<OrganisationType>> GetOrganisationTypes(int providerTypeId)
        {
            return await Get<IEnumerable<OrganisationType>>($"{_baseUrl}/api/v1/lookupData/organisationTypes?providerTypeId={providerTypeId}");
        }

        public async Task<IEnumerable<ProviderType>> GetProviderTypes()
        {
            return await Get<IEnumerable<ProviderType>>($"{_baseUrl}/api/v1/lookupData/providerTypes");
        }
        
        public async Task CreateOrganisation(CreateOrganisationRequest organisationRequest)
        {
            await Post<CreateOrganisationRequest>($"{_baseUrl}/api/v1/organisation/create", organisationRequest);
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

        private async Task Post<T>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            using (var response = await _client.PostAsync(new Uri(uri, UriKind.Absolute),
                new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json"))) ;
        }

    }
}
