using System;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.ExternalApis.Types;

namespace SFA.DAS.AssessorService.ExternalApis
{
    public interface IIlrApiClient
    {
        Task<SearchResponse> Search(SearchRequest request);
    }

    public class IlrApiClient : IIlrApiClient
    {
        public async Task<SearchResponse> Search(SearchRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
