using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<GetStandardByIdResponse>> GetAllStandardDetails(IEnumerable<string> standardUIds)
        {
            var tasks = standardUIds.Select(id => outerApiClient.Get<GetStandardByIdResponse>(new GetStandardByIdRequest(id)));
            await Task.WhenAll(tasks);
            var data = tasks.Select(t => t.Result).ToList();
            return data; 
        }

        public async Task<IEnumerable<GetStandardsListItem>> GetAllStandards()
        {
            var response = await outerApiClient.Get<GetStandardsListResponse>(new GetStandardsRequest());
            return response.Standards;
        }
    }
}
