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

        public async Task<IEnumerable<GetStandardByIdResponse>> GetAllStandardDetails(IEnumerable<string> standardUIds)
        {
            try
            {
                var tasks = standardUIds.Select(id => outerApiClient.Get<GetStandardByIdResponse>(new GetStandardByIdRequest(id))).ToList();
                await Task.WhenAll(tasks);
                var data = tasks.Select(t => t.Result).ToList();
                return data;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while fetching details for all the standards");
                throw;
            }
        }

        public async Task<IEnumerable<GetStandardsListItem>> GetAllStandards()
        {
            var response = await outerApiClient.Get<GetStandardsListResponse>(new GetStandardsRequest());
            return response.Standards;
        }

        public async Task<IEnumerable<GetStandardsListItem>> GetDraftStandards()
        {
            var response = await outerApiClient.Get<GetStandardsListResponse>(new GetDraftStandardsRequest());
            return response.Standards;
        }
    }
}
