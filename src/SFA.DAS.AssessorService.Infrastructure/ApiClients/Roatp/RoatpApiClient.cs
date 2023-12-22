using AutoMapper;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Types.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp
{
    public class RoatpApiClient : IRoatpApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<RoatpApiClient> _logger;
        private readonly ITokenService _tokenService;

        public RoatpApiClient(HttpClient client, IRoatpTokenService tokenService, ILogger<RoatpApiClient> logger)
        {
            _client = client;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<IEnumerable<OrganisationSearchResult>> SearchOrganisationByName(string searchTerm, bool exactMatch)
        {
            _logger.LogInformation($"Searching RoATP. Search Term: {searchTerm}");
            var apiResponse = await Get<AssessorService.Api.Types.Models.Roatp.OrganisationSearchResults>($"/api/v1/search?searchTerm={searchTerm}");

            if (exactMatch && apiResponse?.SearchResults != null)
            {
                apiResponse.SearchResults = apiResponse.SearchResults.Where(r => r.LegalName.Equals(searchTerm, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }

            return Mapper.Map<IEnumerable<SFA.DAS.AssessorService.Api.Types.Models.Roatp.Organisation>, IEnumerable<OrganisationSearchResult>>(apiResponse?.SearchResults);
        }

        public async Task<OrganisationSearchResult> GetOrganisationByUkprn(long ukprn)
        {
            var organisationSearchResults = await GetOrganisationSearchResultsFromRoatp(Convert.ToInt32(ukprn));
            return organisationSearchResults.FirstOrDefault();
        }

        public async Task<IEnumerable<OrganisationSearchResult>> SearchOrganisationByUkprn(int ukprn)
        {
            // Search directly in UKRLP first as there will be more information about the Organisation if it is there.
            var ukrlpResult = await SearchOrganisationInUkrlp(ukprn);

            if (ukrlpResult != null && ukrlpResult.Any())
            {
                return ukrlpResult;
            }
            else
            {
                return await GetOrganisationSearchResultsFromRoatp(ukprn);
            }
        }

        public async Task<IEnumerable<OrganisationSearchResult>> SearchOrganisationInUkrlp(int ukprn)
        {
            _logger.LogInformation($"Searching UKRLP. Ukprn: {ukprn}");
            var apiResponse = await Get<AssessorService.Api.Types.Models.UKRLP.UkprnLookupResponse>($"/api/v1/ukrlp/lookup/{ukprn}");

            return Mapper.Map<IEnumerable<AssessorService.Api.Types.Models.UKRLP.ProviderDetails>, IEnumerable<OrganisationSearchResult>>(apiResponse?.Results);
        }

        private async Task<T> Get<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _tokenService.GetTokenAsync());

            using (var response = await _client.GetAsync(new Uri(uri, UriKind.Relative)))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<T>();
                }

                return default(T);
            }
        }

        private async Task<IEnumerable<OrganisationSearchResult>> GetOrganisationSearchResultsFromRoatp(int ukprn)
        {
            _logger.LogInformation($"Searching RoATP. UKPRN: {ukprn}");
            var apiResponse =
                await Get<AssessorService.Api.Types.Models.Roatp.OrganisationSearchResults>(
                    $"/api/v1/search?searchTerm={ukprn}");

            if (apiResponse?.SearchResults != null)
            {
                apiResponse.SearchResults = apiResponse.SearchResults
                    .Where(r => r.UKPRN.Equals(ukprn.ToString(), StringComparison.InvariantCultureIgnoreCase)).ToList();
            }

            var organisationSearchResults =
                Mapper
                    .Map<IEnumerable<SFA.DAS.AssessorService.Api.Types.Models.Roatp.Organisation>,
                        IEnumerable<OrganisationSearchResult>>(apiResponse?.SearchResults);
            return organisationSearchResults;
        }
    }
}
