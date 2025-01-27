using AutoMapper;
using CharityCommissionService;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.CharityCommission
{
    /// <summary>
    /// Charity Commission API docs are located at: http://apps.charitycommission.gov.uk/Showcharity/API/SearchCharitiesV1/Docs/DevGuideHome.aspx
    /// Charity Commission WSDL is located at: https://apps.charitycommission.gov.uk/Showcharity/API/SearchCharitiesV1/SearchCharitiesV1.asmx?WSDL
    /// There is a Web-Friendly version located at: http://beta.charitycommission.gov.uk/charity-search/
    /// </summary>
    public class CharityCommissionApiClient : ICharityCommissionApiClient
    {
        private readonly ISearchCharitiesV1SoapClient _client;
        private readonly ILogger<CharityCommissionApiClient> _logger;
        private readonly ICharityCommissionApiClientConfiguration _configuration;
        private readonly IMapper _mapper;

        public CharityCommissionApiClient(ISearchCharitiesV1SoapClient client, ILogger<CharityCommissionApiClient> logger, ICharityCommissionApiClientConfiguration configuration, IMapper mapper)
        {
            _client = client;
            _logger = logger;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<AssessorService.Api.Types.CharityCommission.Charity> GetCharity(int charityNumber)
        {
            var charity = await GetCharityDetails(charityNumber);

            if (charity != null)
            {
                // nothing to do at the moment
            }

            return charity;
        }

        public async Task<bool> IsCharityActivelyTrading(int charityNumber)
        {
            var isTrading = false;

            var charity = await GetCharityDetails(charityNumber);

            if (charity != null)
            {
                isTrading = "registered".Equals(charity.Status, StringComparison.InvariantCultureIgnoreCase) && charity.DissolvedOn == null;
            }

            return isTrading;
        }

        private async Task<SFA.DAS.AssessorService.Api.Types.CharityCommission.Charity> GetCharityDetails(int charityNumber)
        {
            _logger.LogInformation($"Searching Charity Commission - Charity Details. Charity Number: {charityNumber}");
            var request = new GetCharityByRegisteredCharityNumberRequest(_configuration.ApiKey, charityNumber);
            var apiResponse = await _client.GetCharityByRegisteredCharityNumberAsync(request);
            return _mapper.Map<Charity, AssessorService.Api.Types.CharityCommission.Charity>(apiResponse.GetCharityByRegisteredCharityNumberResult);
        }
    }
}
