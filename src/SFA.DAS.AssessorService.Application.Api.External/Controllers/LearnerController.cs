using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.External.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using SFA.DAS.AssessorService.Application.Api.External.Models.Internal;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Learner;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Examples;
using System.Linq;
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


        [HttpGet("{uln}/{familyName}/{*standard}")]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(SwaggerHelpers.Examples.GetLearnerExample))]
        [SwaggerResponse((int)HttpStatusCode.OK, "The current Leaner.", typeof(GetLearner))]
        [SwaggerResponseExample((int)HttpStatusCode.Forbidden, typeof(SwaggerHelpers.Examples.ApiResponseExample))]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, "There are validation errors preventing you from retrieving the Learner.", typeof(ApiResponse))]
        [SwaggerOperation("Get Learner", "Gets the specified Learner.", Produces = new string[] { "application/json" })]
        public async Task<IActionResult> GetLearner(long uln, string familyName, [SwaggerParameter("Standard Code or Standard Reference Number")] string standard)
        {
            var getRequest = new GetBatchLearnerRequest { UkPrn = _headerInfo.Ukprn, Email = _headerInfo.Email, Uln = uln, FamilyName = familyName, Standard = standard };
            var response = await _apiClient.GetLearner(getRequest);

            if (response.ValidationErrors.Any())
            {
                ApiResponse error = new ApiResponse((int)HttpStatusCode.Forbidden, string.Join("; ", response.ValidationErrors));
                return StatusCode(error.StatusCode, error);
            }
            else if (response.Certificate != null)
            {
                return Ok(response.Certificate);
            }
            else
            {
                return Ok(response.Learner);
            }
        }
    }
}