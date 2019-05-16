using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using SFA.DAS.AssessorService.Settings;
using UKRLP;

namespace SFA.DAS.AssessorService.Application.Api.Clients
{
    public class UkrlpClient : IUkrlpClient
    {
        private readonly ProviderQueryPortType _client;

        private readonly ILogger<UkrlpClient> _logger;

        private readonly IWebConfiguration _webConfiguration;

        public UkrlpClient(ProviderQueryPortType client, ILogger<UkrlpClient> logger, IWebConfiguration webConfiguration)
        {
            _client = client;
            _logger = logger;
            _webConfiguration = webConfiguration;
        }

        public async Task<List<ProviderDetails>> GetTrainingProviderByUkprn(long ukprn)
        {
            var searchCriteria = new SelectionCriteriaStructure
            {
                ApprovedProvidersOnly = YesNoType.No,
                ApprovedProvidersOnlySpecified = true,
                UnitedKingdomProviderReferenceNumberList = new[] { ukprn.ToString() },
                StakeholderId = _webConfiguration.UkrlpApiAuthentication.StakeholderId,
                ProviderStatus = "A",
                CriteriaCondition = QueryCriteriaConditionType.OR,
                CriteriaConditionSpecified = true
            };

            var query = new ProviderQueryStructure
            {
                QueryId = _webConfiguration.UkrlpApiAuthentication.QueryId,
                SelectionCriteria = searchCriteria
            };

            var request = new ProviderQueryParam(query);

            var searchResult = await _client.retrieveAllProvidersAsync(request);

            var providerDetails = new List<ProviderDetails>();

            if (searchResult.ProviderQueryResponse.MatchingProviderRecords != null)
            {
                providerDetails =
                    Mapper.Map<List<ProviderDetails>>(searchResult.ProviderQueryResponse.MatchingProviderRecords);
            }

            return await Task.FromResult(providerDetails);

        }
    }
}
