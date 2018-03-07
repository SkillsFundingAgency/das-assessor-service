using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.ExternalApis.Ilr;
using SFA.DAS.AssessorService.ExternalApis.Ilr.Types;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Route("api/v1/search")]
    public class SearchController : Controller
    {
        private readonly IIlrApiClient _ilrApi;

        public SearchController(IIlrApiClient ilrApi)
        {
            _ilrApi = ilrApi;
        }

        [HttpGet(Name = "Search")]
        public async Task<IActionResult> Search(SearchQuery searchQuery)
        {
            var result = await _ilrApi.Search(new SearchRequest());

            return Ok(result);
        }
    }
}