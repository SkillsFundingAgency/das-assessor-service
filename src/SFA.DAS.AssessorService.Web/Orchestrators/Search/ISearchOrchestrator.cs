using SFA.DAS.AssessorService.Web.ViewModels.Search;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Orchestrators.Search
{
    public interface ISearchOrchestrator
    {
        Task<SearchRequestViewModel> Search(SearchRequestViewModel vm);
    }
}