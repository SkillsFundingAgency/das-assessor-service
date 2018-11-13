using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.External.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.External.Messages;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using SFA.DAS.AssessorService.Application.Api.External.Models.Certificates;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
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

        [HttpGet("{uln}/{familyName}", Name = "Get")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Certificate>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "The specified learner could not be found.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerOperation("Get Learner Details", "Gets the Learner details for the specified Uln and Family Name.")]
        public async Task<IActionResult> Get(long uln, string familyName)
        {
            GetLearnerDetailsRequest searchQuery = new GetLearnerDetailsRequest
            {
                Uln = uln,
                FamilyName = familyName,
                UkPrn = _headerInfo.Ukprn,
                Email = _headerInfo.Email
            };

            IEnumerable<Certificate> results = await _apiClient.Search(searchQuery);

            if (!results.Any())
            {
                return NotFound();
            }
            else
            {
                return Ok(results);
            }
        }

        [HttpGet("{uln}/{familyName}/{standardCode}", Name = "GetByStandardCode")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Certificate>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "The specified learner could not be found.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerOperation("Get Learner Details - Filtered By Standard", "Gets the Learner details for the specified Uln, Family Name and Standard Code.")]
        public async Task<IActionResult> GetByStandardCode(long uln, string familyName, int standardCode)
        {
            GetLearnerDetailsRequest searchQuery = new GetLearnerDetailsRequest
            {
                Uln = uln,
                FamilyName = familyName,
                StandardCode = standardCode,
                UkPrn = _headerInfo.Ukprn,
                Email = _headerInfo.Email                
            };

            IEnumerable<Certificate> results = await _apiClient.Search(searchQuery);

            if (!results.Any())
            {
                return NotFound();
            }
            else
            {
                return Ok(results);
            }
        }
    }
}