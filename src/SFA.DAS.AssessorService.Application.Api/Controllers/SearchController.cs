using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.ViewModel.Models;
using SFA.DAS.ILR.Api.Client;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    public class SearchController : Controller
    {
        private IIlrApiClient _ilrApi;

        public SearchController(IIlrApiClient ilrApi)
        {
            _ilrApi = ilrApi;
        }

        public async Task<IActionResult> Search(SearchQueryViewModel searchQueryViewModel)
        {
            var result = _ilrApi.Search(searchQueryViewModel);

            return Ok();
        }
    }
}