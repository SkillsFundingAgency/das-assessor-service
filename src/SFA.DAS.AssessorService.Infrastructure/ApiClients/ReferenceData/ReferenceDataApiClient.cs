using AutoMapper;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.ReferenceData
{
    public class ReferenceDataApiClient : IReferenceDataApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ReferenceDataApiClient> _logger;

        public ReferenceDataApiClient(IReferenceDataApiClientFactory clientFactory, ILogger<ReferenceDataApiClient> logger)
        {
            _httpClient = clientFactory.CreateHttpClient();
            _logger = logger;
        }

        public async Task<IEnumerable<OrganisationSearchResult>> SearchOrgansiation(string searchTerm, bool exactMatch)
        {
            _logger.LogInformation($"Searching Reference Data API. Search Term: {searchTerm}");
            var apiResponse = await Get<IEnumerable<Api.Types.Models.ReferenceData.Organisation>>($"/api/organisations?searchTerm={searchTerm}");

            if (exactMatch)
            {
                apiResponse = apiResponse?.Where(r => r.Name.Equals(searchTerm, StringComparison.InvariantCultureIgnoreCase)).AsEnumerable();
            }

            return Mapper.Map<IEnumerable<AssessorService.Api.Types.Models.ReferenceData.Organisation>, IEnumerable<OrganisationSearchResult>>(apiResponse);
        }

        private async Task<T> Get<T>(string uri)
        {
            using (var response = await _httpClient.GetAsync(new Uri(uri, UriKind.Relative)))
            {
                return await response.Content.ReadAsAsync<T>();
            }
        }
    }
}
