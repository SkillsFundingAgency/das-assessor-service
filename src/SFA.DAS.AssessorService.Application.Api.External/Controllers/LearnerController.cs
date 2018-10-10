using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.External.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using SFA.DAS.AssessorService.Application.Api.External.Models.Search;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.External.Controllers
{
    [Route("api/v1/learner")]
    [ApiController]
    [SwaggerTag("Learner Details")]
    public class LearnerController : ControllerBase
    {
        private readonly ILogger<LearnerController> _logger;
        private readonly IHeaderInfo _headerInfo;
        private readonly IApiClient _apiClient;

        public LearnerController(ILogger<LearnerController> logger, IHeaderInfo headerInfo, IApiClient apiClient)
        {
            _logger = logger;
            _headerInfo = headerInfo;
            _apiClient = apiClient;
        }

        [HttpGet("{uln}/{familyName}/{standardCode:int?}", Name = "Get")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<SearchResult>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerOperation("Get Learner Details", "Gets the Learner details for the specified Uln and Family Name, filtered by Standard Code if specified.")]
        public async Task<IActionResult> Get(long uln, string familyName, int? standardCode = null)
        {            
            SearchQuery searchQuery = new SearchQuery
            {
                Uln = uln,
                Surname = familyName,
                UkPrn = _headerInfo.Ukprn,
                Username = _headerInfo.Email
            };

            List<SearchResult> results = await _apiClient.Search(searchQuery, standardCode);
            
            return Ok(results);
        }

    }
}