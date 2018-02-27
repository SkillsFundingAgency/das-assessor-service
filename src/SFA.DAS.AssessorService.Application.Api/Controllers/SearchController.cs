﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.ExternalApis;
using SFA.DAS.AssessorService.ExternalApis.Ilr;
using SFA.DAS.AssessorService.ExternalApis.Ilr.Types;
using SFA.DAS.AssessorService.ViewModel.Models;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Route("api/v1/search")]
    public class SearchController : Controller
    {
        private IIlrApiClient _ilrApi;

        public SearchController(IIlrApiClient ilrApi)
        {
            _ilrApi = ilrApi;
        }

        public async Task<IActionResult> Search(SearchQueryViewModel searchQueryViewModel)
        {
            var result = await _ilrApi.Search(new SearchRequest());

            return Ok(result);  
        }
    }
}