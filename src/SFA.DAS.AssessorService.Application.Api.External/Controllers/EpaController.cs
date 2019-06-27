using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.External.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using SFA.DAS.AssessorService.Application.Api.External.Models.Internal;
using SFA.DAS.AssessorService.Application.Api.External.Models.Request;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;
using SFA.DAS.AssessorService.Domain.Consts;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Examples;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.External.Controllers
{
    [Route("api/v1/epa")]
    [ApiController]
    [SwaggerTag("Epa Details")]
    public class EpaController : ControllerBase
    {
        private const int MAX_EPAS_IN_REQUEST = 25;
        private readonly string MAX_EPAS_IN_REQUEST_ERROR_MESSAGE = $"Batch limited to {MAX_EPAS_IN_REQUEST} requests";

        private readonly ILogger<EpaController> _logger;
        private readonly IHeaderInfo _headerInfo;
        private readonly IApiClient _apiClient;

        public EpaController(ILogger<EpaController> logger, IHeaderInfo headerInfo, IApiClient apiClient)
        {
            _logger = logger;
            _headerInfo = headerInfo;
            _apiClient = apiClient;
        }

        [HttpPost]
        [SwaggerRequestExample(typeof(IEnumerable<CreateEpaRequest>), typeof(SwaggerHelpers.Examples.CreateEpaExample))]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(SwaggerHelpers.Examples.CreateEpaResponseExample))]
        [SwaggerResponse((int)HttpStatusCode.OK, "For each item: The created EPA record if valid, else a list of validation errors.", typeof(IEnumerable<CreateCertificateResponse>))]
        [SwaggerResponseExample((int)HttpStatusCode.Forbidden, typeof(SwaggerHelpers.Examples.TooManyRequestsResponseExample))]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, "There are too many EPA records specified within the request.", typeof(ApiResponse))]
        [SwaggerOperation("Create EPA Records", "Creates a initial EPA record for each valid item within the request.", Consumes = new string[] { "application/json" }, Produces = new string[] { "application/json" })]
        public async Task<IActionResult> CreateEpaRecords([FromBody] IEnumerable<CreateEpaRequest> request)
        {
            if(request.Count() > MAX_EPAS_IN_REQUEST)
            {
                ApiResponse error = new ApiResponse((int)HttpStatusCode.Forbidden, MAX_EPAS_IN_REQUEST_ERROR_MESSAGE);
                return StatusCode(error.StatusCode, error);
            }

            var createRequest = request.Select(req =>
                new CreateBatchEpaRequest {
                    UkPrn = _headerInfo.Ukprn,
                    Email = _headerInfo.Email,
                    RequestId = req.RequestId,
                    Learner = req.Learner,
                    Standard = req.Standard,
                    EpaDetails = req.EpaDetails
                });

            var results = await _apiClient.CreateEpas(createRequest);
            return Ok(results);
        }

        [HttpPut]
        [SwaggerRequestExample(typeof(IEnumerable<UpdateEpaRequest>), typeof(SwaggerHelpers.Examples.UpdateEpaExample))]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(SwaggerHelpers.Examples.UpdateEpaResponseExample))]
        [SwaggerResponse((int)HttpStatusCode.OK, "For each item: The updated EPA records if valid, else a list of validation errors.", typeof(IEnumerable<UpdateEpaResponse>))]
        [SwaggerResponseExample((int)HttpStatusCode.Forbidden, typeof(SwaggerHelpers.Examples.TooManyRequestsResponseExample))]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, "There are too many EPA records specified within the request.", typeof(ApiResponse))]
        [SwaggerOperation("Update EPA Records", "Updates the specified EPA record with the information contained in each valid request.", Consumes = new string[] { "application/json" }, Produces = new string[] { "application/json" })]
        public async Task<IActionResult> UpdateEpaRecords([FromBody] IEnumerable<UpdateEpaRequest> request)
        {
            if (request.Count() > MAX_EPAS_IN_REQUEST)
            {
                ApiResponse error = new ApiResponse((int)HttpStatusCode.Forbidden, MAX_EPAS_IN_REQUEST_ERROR_MESSAGE);
                return StatusCode(error.StatusCode, error);
            }

            var updateRequest = request.Select(req =>
                new UpdateBatchEpaRequest
                {
                    UkPrn = _headerInfo.Ukprn,
                    Email = _headerInfo.Email,
                    RequestId = req.RequestId,
                    EpaReference = req.EpaReference,
                    Learner = req.Learner,
                    Standard = req.Standard,
                    EpaDetails = req.EpaDetails
                });

            var results = await _apiClient.UpdateEpas(updateRequest);
            return Ok(results);
        }

        [HttpDelete("{uln}/{familyName}/{standard}/{*epaReference}")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "The specified Epa record has been deleted.")]
        [SwaggerResponseExample((int)HttpStatusCode.Forbidden, typeof(SwaggerHelpers.Examples.ApiResponseExample))]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, "There are validation errors preventing you from deleting the EPA record.", typeof(ApiResponse))]
        [SwaggerOperation("Delete EPA Record", "Deletes the specified EPA record.", Produces = new string[] { "application/json" })]
        public async Task<IActionResult> DeleteCertificate(long uln, string familyName, [SwaggerParameter("Standard Code or Standard Reference Number")] string standard, string epaReference)
        {
            var deleteRequest = new DeleteBatchEpaRequest { UkPrn = _headerInfo.Ukprn, Email = _headerInfo.Email, Uln = uln, FamilyName = familyName, Standard = standard, EpaReference = epaReference};
            var error = await _apiClient.DeleteEpa(deleteRequest);

            if (error is null)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(error);
            }
        }
    }
}