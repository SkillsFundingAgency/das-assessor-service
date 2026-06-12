using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.OuterApi;

namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public class OuterApiService : IOuterApiService
    {
        private readonly IOuterApiClient _outerApiClient;

        public OuterApiService(IOuterApiClient outerApiClient)
        {
            _outerApiClient = outerApiClient;
        }

        public async Task<IEnumerable<GetStandardsListItem>> GetActiveStandards()
        {
            var response = await _outerApiClient.Get<GetStandardsListResponse>(new GetActiveStandardsRequest());
            return response.Standards;
        }       

        public async Task<IEnumerable<StandardDetailResponse>> GetAllStandards()
        {
            var response = await _outerApiClient.Get<GetStandardsExportListResponse>(new GetStandardsRequest());
            return response.Standards;
        }

        public async Task<IEnumerable<GetStandardsListItem>> GetDraftStandards()
        {
            var response = await _outerApiClient.Get<GetStandardsListResponse>(new GetDraftStandardsRequest());
            return response.Standards;
        }

        public async Task<GetAllLearnersResponse> GetAllLearners(DateTime? sinceTime, int batchNumber, int batchSize)
        {
            var response = await _outerApiClient.Get<GetAllLearnersResponse>(new GetAllLearnersRequest(sinceTime, batchNumber, batchSize));
            return response;
        }

        public async Task<GetAddressesResponse> GetAddresses(string query)
        {
            var response = await _outerApiClient.Get<GetAddressesResponse>(new GetAddressesRequest(query));
            return response ?? new GetAddressesResponse();
        }
    }
}
