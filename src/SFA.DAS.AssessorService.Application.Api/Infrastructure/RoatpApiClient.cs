using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Settings;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;

namespace SFA.DAS.AssessorService.Application.Api.Infrastructure
{
    public class RoatpApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<RoatpApiClient> _logger;
        private readonly IWebConfiguration _config;

        public RoatpApiClient(HttpClient client, ILogger<RoatpApiClient> logger, IWebConfiguration configurationService)
        {
            _client = client;
            _logger = logger;
            _config = configurationService;
        }

        public async Task<IEnumerable<OrganisationSearchResult>> SearchOrgansiationByName(string searchTerm, bool exactMatch)
        {
            _logger.LogInformation($"Searching RoATP. Search Term: {searchTerm}");
            var apiResponse = await Get<AssessorService.Api.Types.Models.Roatp.OrganisationSearchResults>($"/api/v1/search?searchTerm={searchTerm}");

            if (exactMatch && apiResponse?.SearchResults != null)
            {
                apiResponse.SearchResults = apiResponse.SearchResults.Where(r => r.LegalName.Equals(searchTerm, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }

            return Mapper.Map<IEnumerable<SFA.DAS.AssessorService.Api.Types.Models.Roatp.Organisation>, IEnumerable<OrganisationSearchResult>>(apiResponse?.SearchResults);
        }

        public async Task<IEnumerable<OrganisationSearchResult>> SearchOrgansiationByUkprn(int ukprn)
        {
            // Search directly in UKRLP first as there will be more information about the Organisation if it is there.
            var ukrlpResult = await SearchOrgansiationInUkrlp(ukprn);

            if (ukrlpResult != null && ukrlpResult.Any())
            {
                return ukrlpResult;
            }
            else
            {
                // If we get a result here then it means the Organisation is in the process of On-Boarding
                _logger.LogInformation($"Searching RoATP. UKPRN: {ukprn}");
                var apiResponse = await Get<AssessorService.Api.Types.Models.Roatp.OrganisationSearchResults>($"/api/v1/search?searchTerm={ukprn}");

                if (apiResponse?.SearchResults != null)
                {
                    apiResponse.SearchResults = apiResponse.SearchResults.Where(r => r.UKPRN.Equals(ukprn.ToString(), StringComparison.InvariantCultureIgnoreCase)).ToList();
                }

                return Mapper.Map<IEnumerable<SFA.DAS.AssessorService.Api.Types.Models.Roatp.Organisation>, IEnumerable<OrganisationSearchResult>>(apiResponse?.SearchResults);
            }
        }

        private async Task<IEnumerable<OrganisationSearchResult>> SearchOrgansiationInUkrlp(int ukprn)
        {
            _logger.LogInformation($"Searching UKRLP. Ukprn: {ukprn}");
            var apiResponse = await Get<AssessorService.Api.Types.Models.UKRLP.UkprnLookupResponse>($"/api/v1/ukrlp/lookup/{ukprn}");

            return Mapper.Map<IEnumerable<AssessorService.Api.Types.Models.UKRLP.ProviderDetails>, IEnumerable<OrganisationSearchResult>>(apiResponse?.Results);
        }

        private async Task<T> Get<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());

            using (var response = await _client.GetAsync(new Uri(uri, UriKind.Relative)))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<T>();
                }

                return default;
            }
        }

        private string GetToken()
        {
            var tenantId = _config.RoatpApiAuthentication.TenantId;
            var clientId = _config.RoatpApiAuthentication.ClientId;
            var clientSecret = _config.RoatpApiAuthentication.ClientSecret;
            var resourceId = _config.RoatpApiAuthentication.ResourceId;

            var authority = $"https://login.microsoftonline.com/{tenantId}";
            var clientCredential = new ClientCredential(clientId, clientSecret);
            var context = new AuthenticationContext(authority, true);
            var result = context.AcquireTokenAsync(resourceId, clientCredential).Result;

            return result.AccessToken;
        }
    }
}
