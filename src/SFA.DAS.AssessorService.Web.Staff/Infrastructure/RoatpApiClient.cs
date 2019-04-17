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
    using System.Net;

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

        public async Task<IEnumerable<OrganisationStatus>> GetOrganisationStatuses(int? providerTypeId)
        {
            return await Get<IEnumerable<OrganisationStatus>>($"{_baseUrl}/api/v1/lookupData/organisationStatuses?providerTypeId={providerTypeId}");
        }

        public async Task<IEnumerable<RemovedReason>> GetRemovedReasons()
        {
            return await Get<IEnumerable<RemovedReason>>($"{_baseUrl}/api/v1/lookupData/removedReasons");
        }

        public async Task<bool> CreateOrganisation(CreateOrganisationRequest organisationRequest)
        {
           HttpStatusCode result = await Post<CreateOrganisationRequest>($"{_baseUrl}/api/v1/organisation/create", organisationRequest);

           return await Task.FromResult(result == HttpStatusCode.OK);
        }
        
        public async Task<DuplicateCheckResponse> DuplicateUKPRNCheck(Guid organisationId, long ukprn)
        {
            return await Get<DuplicateCheckResponse>($"{_baseUrl}/api/v1/duplicateCheck/ukprn?ukprn={ukprn}&organisationId={organisationId}");
        }

        public async Task<DuplicateCheckResponse> DuplicateCompanyNumberCheck(Guid organisationId, string companyNumber)
        {
            return await Get<DuplicateCheckResponse>($"{_baseUrl}/api/v1/duplicateCheck/companyNumber?companyNumber={companyNumber}&organisationId={organisationId}");
        }

        public async Task<DuplicateCheckResponse> DuplicateCharityNumberCheck(Guid organisationId, string charityNumber)
        {
            return await Get<DuplicateCheckResponse>($"{_baseUrl}/api/v1/duplicateCheck/charityNumber?charityNumber={charityNumber}&organisationId={organisationId}");
        }

        public async Task<OrganisationSearchResults> Search(string searchTerm)
        {
            return await Get<OrganisationSearchResults>($"{_baseUrl}/api/v1/search?searchTerm={searchTerm}");
        }

        public async Task<bool> UpdateOrganisationLegalName(UpdateOrganisationLegalNameRequest request)
        {
            HttpStatusCode result = await Put<UpdateOrganisationLegalNameRequest>($"{_baseUrl}/api/v1/updateOrganisation/legalName", request);

            return await Task.FromResult(result == HttpStatusCode.OK);
        }
        
        public async Task<bool> UpdateOrganisationStatus(UpdateOrganisationStatusRequest request)
        {
            HttpStatusCode result = await Put<UpdateOrganisationStatusRequest>($"{_baseUrl}/api/v1/updateOrganisation/status", request);

            return await Task.FromResult(result == HttpStatusCode.OK);
        }

        public async Task<bool> UpdateOrganisationTradingName(UpdateOrganisationTradingNameRequest request)
        {
            HttpStatusCode result = await Put<UpdateOrganisationTradingNameRequest>($"{_baseUrl}/api/v1/updateOrganisation/tradingName", request);

            return await Task.FromResult(result == HttpStatusCode.OK);
        }
        
        public async Task<bool> UpdateOrganisationParentCompanyGuarantee(UpdateOrganisationParentCompanyGuaranteeRequest request)
        {
            HttpStatusCode result = await Put<UpdateOrganisationParentCompanyGuaranteeRequest>($"{_baseUrl}/api/v1/updateOrganisation/parentCompanyGuarantee", request);

            return await Task.FromResult(result == HttpStatusCode.OK);
        }
       
        public async Task<bool> UpdateOrganisationFinancialTrackRecord(UpdateOrganisationFinancialTrackRecordRequest request)
        {
            HttpStatusCode result = await Put<UpdateOrganisationFinancialTrackRecordRequest>($"{_baseUrl}/api/v1/updateOrganisation/financialTrackRecord", request);

            return await Task.FromResult(result == HttpStatusCode.OK);
        }

        public async Task<bool> UpdateOrganisationProviderType(UpdateOrganisationProviderTypeRequest request)
        {
            HttpStatusCode result = await Put<UpdateOrganisationProviderTypeRequest>($"{_baseUrl}/api/v1/updateOrganisation/providerType", request);

            return await Task.FromResult(result == HttpStatusCode.OK);
        }

        public async Task<bool> UpdateOrganisationUkprn(UpdateOrganisationUkprnRequest request)
        {
            HttpStatusCode result = await Put<UpdateOrganisationUkprnRequest>($"{_baseUrl}/api/v1/updateOrganisation/ukprn", request);

            return await Task.FromResult(result == HttpStatusCode.OK);
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

        private async Task<HttpStatusCode> Post<T>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            var response = await _client.PostAsync(new Uri(uri, UriKind.Absolute),
                new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json"));

             return response.StatusCode;
        }

        private async Task<HttpStatusCode> Put<T>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            var response = await _client.PutAsync(new Uri(uri, UriKind.Absolute),
                new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json"));

            return response.StatusCode;
        }

        private async Task<U> Post<T, U>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            var response = await _client.PostAsync(new Uri(uri, UriKind.Absolute),
                new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json"));

            return await response.Content.ReadAsAsync<U>();
        }

    }
}
