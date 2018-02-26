using System;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.ExternalApis.Ilr.Types;

namespace SFA.DAS.AssessorService.ExternalApis.Ilr
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
