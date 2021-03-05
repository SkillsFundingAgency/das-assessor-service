using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Application.Infrastructure.OuterApi;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public class OuterApiService : IOuterApiService
    {
        private readonly IOuterApiClient outerApiClient;

        public OuterApiService(IOuterApiClient outerApiClient)
        {
            this.outerApiClient = outerApiClient;
        }

        public async Task<IEnumerable<GetStandardsListItem>> GetAllStandards()
        {
            var response = await outerApiClient.Get<GetStandardsListResponse>(new GetStandardsRequest());
            return response.Standards;
        }
    }
}
