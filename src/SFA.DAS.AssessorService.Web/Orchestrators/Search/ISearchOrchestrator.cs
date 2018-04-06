using System.Threading.Tasks;
using SFA.DAS.AssessorService.Web.ViewModels.Search;

namespace SFA.DAS.AssessorService.Web.Orchestrators.Search
{
    public interface ISearchOrchestrator
    {
        Task<SearchRequestViewModel> Search(SearchRequestViewModel vm);
    }
}