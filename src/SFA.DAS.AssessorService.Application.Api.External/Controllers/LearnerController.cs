using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.External.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using SFA.DAS.AssessorService.Application.Api.External.Models.Search;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.External.Controllers
{
    [Route("api/v1/learner")]
    [ApiController]
    public class LearnerController : ControllerBase
    {
        private readonly ILogger<LearnerController> _logger;
        private readonly IHeaderInfo _headerInfo;
        private readonly ApiClient _apiClient;

        public LearnerController(ILogger<LearnerController> logger, IHeaderInfo headerInfo, ApiClient apiClient)
        {
            _logger = logger;
            _headerInfo = headerInfo;
            _apiClient = apiClient;
        }

        [HttpGet("{uln}/{lastname}/{stdCode:int?}", Name = "Get")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<SearchResult>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Get(long uln, string lastname, int? stdCode = null)
        {            
            SearchQuery searchQuery = new SearchQuery
            {
                Uln = uln,
                Surname = lastname,
                UkPrn = _headerInfo.Ukprn,
                Username = _headerInfo.Username
            };

            List<SearchResult> results = await _apiClient.Search(searchQuery, stdCode);
            
            return Ok(results);
        }

    }
}