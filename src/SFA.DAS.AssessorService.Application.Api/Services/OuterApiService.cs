using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Infrastructure.OuterApi;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public class OuterApiService : IOuterApiService
    {
        private readonly IOuterApiClient outerApiClient;
        private readonly ILogger<OuterApiService> logger;

        public OuterApiService(IOuterApiClient outerApiClient, ILogger<OuterApiService> logger)
        {
            this.outerApiClient = outerApiClient;
            this.logger = logger;
        }

        public async Task<IEnumerable<GetStandardsListItem>> GetActiveStandards()
        {
            var response = await outerApiClient.Get<GetStandardsListResponse>(new GetActiveStandardsRequest());
            return response.Standards;
        }       

        public async Task<IEnumerable<StandardDetailResponse>> GetAllStandards()
        {
            var response = await outerApiClient.Get<GetStandardsExportListResponse>(new GetStandardsRequest());
            return response.Standards;
        }

        public async Task<IEnumerable<GetStandardsListItem>> GetDraftStandards()
        {
            var response = await outerApiClient.Get<GetStandardsListResponse>(new GetDraftStandardsRequest());
            return response.Standards;
        }
    }
}
