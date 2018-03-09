using System.Threading.Tasks;
using SFA.DAS.AssessorService.ExternalApis.Ilr.Types;

namespace SFA.DAS.AssessorService.ExternalApis.Ilr
{
    public interface IIlrApiClient
    {
        Task<SearchResponse> Search(IlrSearchRequest request);
    }
}
