using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp.Types;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp
{
    public class RoatpApiClient : IRoatpApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<RoatpApiClient> _logger;
        private readonly IMapper _mapper;

        public RoatpApiClient(IRoatpApiClientFactory roatpApiClientFactory, ILogger<RoatpApiClient> logger, IMapper mapper)
        {
            _client = roatpApiClientFactory.CreateHttpClient();
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<IEnumerable<OrganisationSearchResult>> SearchOrganisationByName(string searchTerm, bool exactMatch)
        {
            _logger.LogInformation($"Searching RoATP. Search Term: {searchTerm}");
            var apiResponse = await Get<OrganisationSearchResults>($"/organisations?searchTerm={searchTerm}");

            if (exactMatch && apiResponse?.Organisations != null)
            {
                apiResponse.Organisations = apiResponse.Organisations.Where(r => r.LegalName.Equals(searchTerm, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }

            return _mapper.Map<IEnumerable<Organisation>, IEnumerable<OrganisationSearchResult>>(apiResponse?.Organisations);
        }

        public async Task<OrganisationSearchResult> GetOrganisationByUkprn(long ukprn)
        {
            return await GetOrganisationSearchResultsFromRoatp(Convert.ToInt32(ukprn));
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
                var org = await GetOrganisationSearchResultsFromRoatp(ukprn);
                return [org];
            }
        }

        public async Task<IEnumerable<OrganisationSearchResult>> SearchOrganisationInUkrlp(int ukprn)
        {
            _logger.LogInformation("Searching UKRLP. Ukprn: {Ukprn}", ukprn);
            var apiResponse = await Get<UkprnLookupResponse>($"/organisations/{ukprn}/ukrlp-data");

            return _mapper.Map<IEnumerable<ProviderDetails>, IEnumerable<OrganisationSearchResult>>(apiResponse?.Results);
        }

        private async Task<T> Get<T>(string uri)
        {

            using (var response = await _client.GetAsync(new Uri(uri, UriKind.Relative)))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<T>();
                }

                return default(T);
            }
        }

        private async Task<OrganisationSearchResult> GetOrganisationSearchResultsFromRoatp(int ukprn)
        {
            _logger.LogInformation("Searching RoATP. UKPRN: {Ukprn}", ukprn);
            var apiResponse =
                await Get<Organisation>(
                    $"/organisations/{ukprn}");

            var organisationSearchResults =
                _mapper
                    .Map<Organisation, OrganisationSearchResult>(apiResponse);
            return organisationSearchResults;
        }
    }
}
