namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    using System.Threading.Tasks;
    using AssessorService.Api.Types.Models;
    using ExternalApis.Ilr;
    using ExternalApis.Ilr.Types;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/v1/search")]
    public class SearchController : Controller
    {
        private readonly IIlrApiClient _ilrApi;

        public SearchController(IIlrApiClient ilrApi)
        {
            _ilrApi = ilrApi;
        }

        [HttpGet(Name = "Search")]
        public async Task<IActionResult> Search(SearchQueryViewModel searchQueryViewModel)
        {
            var result = await _ilrApi.Search(new SearchRequest());

            return Ok(result);
        }
    }
}