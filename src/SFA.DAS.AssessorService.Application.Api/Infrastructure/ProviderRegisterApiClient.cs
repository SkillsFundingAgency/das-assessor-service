using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Infrastructure
{
    public class ProviderRegisterApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ProviderRegisterApiClient> _logger;

        public ProviderRegisterApiClient(HttpClient client, ILogger<ProviderRegisterApiClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<IEnumerable<AssessorService.Api.Types.Models.OrganisationSearchResult>> SearchOrgansiationByName(string name, bool exactMatch)
        {
            _logger.LogInformation($"Searching Provider Register. Name: {name}");
            var apiResponse = await Get<IEnumerable<AssessorService.Api.Types.Models.ProviderRegister.Provider>>($"/providers/search?keywords={name}");

            if (exactMatch)
            {
                apiResponse = apiResponse?.Where(r => r.ProviderName.Equals(name, StringComparison.InvariantCultureIgnoreCase)).AsEnumerable();
            }

            return Mapper.Map<IEnumerable<SFA.DAS.AssessorService.Api.Types.Models.ProviderRegister.Provider>, 
                IEnumerable<SFA.DAS.AssessorService.Api.Types.Models.OrganisationSearchResult>>(apiResponse);
        }

        public async Task<SFA.DAS.AssessorService.Api.Types.Models.OrganisationSearchResult> SearchOrgansiationByUkprn(int ukprn)
        {
            _logger.LogInformation($"Searching Provider Register. Ukprn: {ukprn}");
            var apiResponse = await Get<SFA.DAS.AssessorService.Api.Types.Models.ProviderRegister.Provider>($"/providers/{ukprn}");

            return Mapper.Map<SFA.DAS.AssessorService.Api.Types.Models.ProviderRegister.Provider, 
                SFA.DAS.AssessorService.Api.Types.Models.OrganisationSearchResult>(apiResponse);
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

    }
}
