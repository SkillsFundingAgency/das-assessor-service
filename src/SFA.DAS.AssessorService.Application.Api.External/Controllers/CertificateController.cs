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
    [Route("api/v1/certificate")]
    [ApiController]
    [SwaggerTag("Batch Certificates")]
    public class CertificateController : ControllerBase
    {
        private readonly ILogger<CertificateController> _logger;
        private readonly IHeaderInfo _headerInfo;
        private readonly IApiClient _apiClient;

        public CertificateController(ILogger<CertificateController> logger, IHeaderInfo headerInfo, IApiClient apiClient)
        {
            _logger = logger;
            _headerInfo = headerInfo;
            _apiClient = apiClient;
        }

        [HttpGet("{uln}/{familyName}/{standardCode}/{certificateReference}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "The specified Certificate.", typeof(Certificate))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "The specified Certificate could not be found.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerOperation("Get Certificate", "Gets the specified Certificate.")]
        public async Task<IActionResult> GetCertificate(long uln, string familyName, int standardCode, string certificateReference)
        {
            GetCertificateRequest getRequest = new GetCertificateRequest { UkPrn = _headerInfo.Ukprn, Email = _headerInfo.Email, Uln = uln, FamilyName = familyName, StandardCode = standardCode, CertificateReference = certificateReference };
            var response = await _apiClient.GetCertificate(getRequest);

            if (response.ValidationErrors.Any())
            {
                return NotFound(string.Join("; ", response.ValidationErrors));
            }
            else
            {
                return Ok(response.Certificate);
            }
        }

        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, "For each item: The created Certificate if valid, else a list of validation errors.", typeof(IEnumerable<BatchCertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerOperation("Create Certificates", "Creates a new Certificate for each valid item within the request.")]
        public async Task<IActionResult> CreateCertificates([FromBody] IEnumerable<CertificateData> request)
        {
            IEnumerable<BatchCertificateRequest> bcRequest = request.Select(req => new BatchCertificateRequest { UkPrn = _headerInfo.Ukprn, Email = _headerInfo.Email, CertificateData = req });

            var results = await _apiClient.CreateCertificates(bcRequest);

            return Ok(results);
        }

        [HttpPut]
        [SwaggerResponse((int)HttpStatusCode.OK, "For each item: The updated Certificate if valid, else a list of validation errors.", typeof(IEnumerable<BatchCertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerOperation("Update Certificates", "Updates the specified Certificate with the information contained in each valid request.")]
        public async Task<IActionResult> UpdateCertificates([FromBody] IEnumerable<CertificateData> request)
        {
            IEnumerable<BatchCertificateRequest> bcRequest = request.Select(req => new BatchCertificateRequest { UkPrn = _headerInfo.Ukprn, Email = _headerInfo.Email, CertificateData = req });

            var results = await _apiClient.UpdateCertificates(bcRequest);

            return Ok(results);
        }

        [HttpPost("submit")]
        [SwaggerResponse((int)HttpStatusCode.OK, "For each item: A the submitted Certificate if valid, else a list of validation errors.", typeof(IEnumerable<SubmitBatchCertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerOperation("Submit Certificates", "Submits the specified Certificate for each valid request.")]
        public async Task<IActionResult> SubmitCertificates([FromBody] IEnumerable<SubmitCertificate> request)
        {
            IEnumerable<SubmitBatchCertificateRequest> scRequest = request.Select(req => new SubmitBatchCertificateRequest { UkPrn = _headerInfo.Ukprn, Email = _headerInfo.Email, Uln = req.Uln, StandardCode = req.StandardCode, FamilyName = req.FamilyName, CertificateReference = req.CertificateReference });

            var results = await _apiClient.SubmitCertificates(scRequest);

            return Ok(results);
        }

        [HttpDelete("{uln}/{familyName}/{standardCode}/{certificateReference}")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "The specified Certificate has been deleted.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerOperation("Delete Certificate", "Deletes the specified Certificate.")]
        public async Task<IActionResult> DeleteCertificate(long uln, string familyName, int standardCode, string certificateReference)
        {
            DeleteCertificateRequest deleteRequest = new DeleteCertificateRequest { UkPrn = _headerInfo.Ukprn, Email = _headerInfo.Email, Uln = uln, FamilyName = familyName, StandardCode = standardCode, CertificateReference = certificateReference};
            var error = await _apiClient.DeleteCertificate(deleteRequest);

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