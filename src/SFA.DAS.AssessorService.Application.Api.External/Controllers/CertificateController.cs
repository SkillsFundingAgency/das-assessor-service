using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.External.Helpers;
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
    [Route("api/v1/certificate")]
    [ApiController]
    [SwaggerTag("Batch Certificates")]
    public class CertificateController : ControllerBase
    {
        private const int MAX_CERTIFICATES_IN_REQUEST = 25;
        private readonly string MAX_CERTIFICATES_IN_REQUEST_ERROR_MESSAGE = $"Batch limited to {MAX_CERTIFICATES_IN_REQUEST} requests";

        private readonly ILogger<CertificateController> _logger;
        private readonly IHeaderInfo _headerInfo;
        private readonly IApiClient _apiClient;

        public CertificateController(ILogger<CertificateController> logger, IHeaderInfo headerInfo, IApiClient apiClient)
        {
            _logger = logger;
            _headerInfo = headerInfo;
            _apiClient = apiClient;
        }

        [HttpGet("{uln}/{familyName}/{*standard}")]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(SwaggerHelpers.Examples.GetCertificateExample))]
        [SwaggerResponse((int)HttpStatusCode.OK, "The current Certificate.", typeof(Certificate))]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "There is no Certificate and you may create one.")]
        [SwaggerResponseExample((int)HttpStatusCode.Forbidden, typeof(SwaggerHelpers.Examples.ApiResponseExample))]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, "There are validation errors preventing you from retrieving the Certificate.", typeof(ApiResponse))]
        [SwaggerOperation("Get Certificate", "Gets the specified Certificate.", Produces = new string[] { "application/json" })]
        public async Task<IActionResult> GetCertificate(long uln, string familyName, [SwaggerParameter("Standard Code or Standard Reference Number")] string standard)
        {
            var getRequest = new GetBatchCertificateRequest { UkPrn = _headerInfo.Ukprn, Uln = uln, FamilyName = familyName, Standard = standard };
            
            var response = await _apiClient.GetCertificate(getRequest);
            
            if (response.ValidationErrors.Any())
            {
                ApiResponse error = new ApiResponse((int)HttpStatusCode.Forbidden, string.Join("; ", response.ValidationErrors));
                return StatusCode(error.StatusCode, error);
            }
            else if(response.Certificate is null)
            {
                return NoContent();
            }
            
            if (CertificateStatus.HasPrintProcessStatus(response.Certificate.Status.CurrentStatus))
            {
                if (response.Certificate.Status.CurrentStatus == CertificateStatus.Printed)
                {
                    response.Certificate.Delivered = new Delivered { Status = "WaitingForDelivery" };
                }
                else
                {
                    var logsResponse = await _apiClient.GetCertificateLogs(response.Certificate.CertificateData.CertificateReference);

                    var deliveryLogs = logsResponse.CertificateLogs.Where(log => log.Status == CertificateStatus.Delivered || log.Status == CertificateStatus.NotDelivered);

                    response.Certificate.Delivered = Mapper.Map<CertificateLog, Delivered>(deliveryLogs.OrderByDescending(log => log.EventTime).FirstOrDefault());
                }

                response.Certificate.Status.CurrentStatus = CertificateStatus.Submitted;
            }
            else // status could be Draft or Deleted (or Privately Funded statuses)
			{
                var certificateData = response.Certificate.CertificateData;

                if (!string.IsNullOrEmpty(certificateData.Standard?.StandardReference) && !string.IsNullOrEmpty(certificateData?.LearningDetails?.Version))
				{
                    var standardOptions = await _apiClient.GetStandardOptionsByStandardIdAndVersion(certificateData.Standard.StandardReference, certificateData.LearningDetails.Version);

                    var hasOptions = standardOptions != null && standardOptions.CourseOption?.Count() > 0;

                    if (CertificateHelpers.IsDraftCertificateDeemedAsReady(response.Certificate, hasOptions))
                    {
                        response.Certificate.Status.CurrentStatus = CertificateStatus.Ready;
                    }
                }
            }

            return Ok(response.Certificate);
        }

        [HttpPost]
        [SwaggerRequestExample(typeof(IEnumerable<CreateCertificateRequest>), typeof(SwaggerHelpers.Examples.CreateCertificateExample))]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(SwaggerHelpers.Examples.CreateCertificateResponseExample))]
        [SwaggerResponse((int)HttpStatusCode.OK, "For each item: The created Certificate if valid, else a list of validation errors.", typeof(IEnumerable<CreateCertificateResponse>))]
        [SwaggerResponseExample((int)HttpStatusCode.Forbidden, typeof(SwaggerHelpers.Examples.TooManyRequestsResponseExample))]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, "There are too many certificates specified within the request.", typeof(ApiResponse))]
        [SwaggerOperation("Create Certificates", "Creates a new Certificate for each valid item within the request.", Consumes = new string[] { "application/json" }, Produces = new string[] { "application/json" })]
        public async Task<IActionResult> CreateCertificates([FromBody] IEnumerable<CreateCertificateRequest> request)
        {
            if(request.Count() > MAX_CERTIFICATES_IN_REQUEST)
            {
                ApiResponse error = new ApiResponse((int)HttpStatusCode.Forbidden, MAX_CERTIFICATES_IN_REQUEST_ERROR_MESSAGE);
                return StatusCode(error.StatusCode, error);
            }

            var createRequest = request.Select(req =>
                new CreateBatchCertificateRequest {
                    UkPrn = _headerInfo.Ukprn,
                    RequestId = req.RequestId,
                    CertificateData = new Models.Request.Certificates.CertificateData
                    {
                        Standard = req.Standard,
                        Learner = req.Learner,
                        LearningDetails = req.LearningDetails,
                        PostalContact = req.PostalContact
                    }
                });

            var results = await _apiClient.CreateCertificates(createRequest);

            foreach(var result in results)
            {
                if (CertificateHelpers.IsDraftCertificateDeemedAsReady(result.Certificate))
                {
                    result.Certificate.Status.CurrentStatus = CertificateStatus.Ready;
                }
            }

            return Ok(results);
        }

        [HttpPut]
        [SwaggerRequestExample(typeof(IEnumerable<UpdateCertificateRequest>), typeof(SwaggerHelpers.Examples.UpdateCertificateExample))]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(SwaggerHelpers.Examples.UpdateCertificateResponseExample))]
        [SwaggerResponse((int)HttpStatusCode.OK, "For each item: The updated Certificate if valid, else a list of validation errors.", typeof(IEnumerable<UpdateCertificateResponse>))]
        [SwaggerResponseExample((int)HttpStatusCode.Forbidden, typeof(SwaggerHelpers.Examples.TooManyRequestsResponseExample))]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, "There are too many certificates specified within the request.", typeof(ApiResponse))]
        [SwaggerOperation("Update Certificates", "Updates the specified Certificate with the information contained in each valid request.", Consumes = new string[] { "application/json" }, Produces = new string[] { "application/json" })]
        public async Task<IActionResult> UpdateCertificates([FromBody] IEnumerable<UpdateCertificateRequest> request)
        {
            if (request.Count() > MAX_CERTIFICATES_IN_REQUEST)
            {
                ApiResponse error = new ApiResponse((int)HttpStatusCode.Forbidden, MAX_CERTIFICATES_IN_REQUEST_ERROR_MESSAGE);
                return StatusCode(error.StatusCode, error);
            }

            var updateRequest = request.Select(req =>
                new UpdateBatchCertificateRequest
                {
                    UkPrn = _headerInfo.Ukprn,
                    RequestId = req.RequestId,
                    CertificateData = new Models.Request.Certificates.CertificateData
                    {
                        CertificateReference = req.CertificateReference,
                        Standard = req.Standard,
                        Learner = req.Learner,
                        LearningDetails = req.LearningDetails,
                        PostalContact = req.PostalContact
                    }
                });

            var results = await _apiClient.UpdateCertificates(updateRequest);

            foreach (var result in results)
            {
                if (CertificateHelpers.IsDraftCertificateDeemedAsReady(result.Certificate))
                {
                    result.Certificate.Status.CurrentStatus = CertificateStatus.Ready;
                }
            }

            return Ok(results);
        }

        [HttpPost("submit")]
        [SwaggerRequestExample(typeof(IEnumerable<SubmitCertificateRequest>), typeof(SwaggerHelpers.Examples.SubmitCertificateExample))]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(SwaggerHelpers.Examples.SubmitCertificateResponseExample))]
        [SwaggerResponse((int)HttpStatusCode.OK, "For each item: The submitted Certificate if valid, else a list of validation errors.", typeof(IEnumerable<SubmitCertificateResponse>))]
        [SwaggerResponseExample((int)HttpStatusCode.Forbidden, typeof(SwaggerHelpers.Examples.TooManyRequestsResponseExample))]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, "There are too many certificates specified within the request.", typeof(ApiResponse))]
        [SwaggerOperation("Submit Certificates", "Submits the specified Certificate for each valid request.", Consumes = new string[] { "application/json" }, Produces = new string[] { "application/json" })]
        public async Task<IActionResult> SubmitCertificates([FromBody] IEnumerable<SubmitCertificateRequest> request)
        {
            if (request.Count() > MAX_CERTIFICATES_IN_REQUEST)
            {
                ApiResponse error = new ApiResponse((int)HttpStatusCode.Forbidden, MAX_CERTIFICATES_IN_REQUEST_ERROR_MESSAGE);
                return StatusCode(error.StatusCode, error);
            }

            var submitRequest = request.Select(req =>
                new SubmitBatchCertificateRequest
                {
                    UkPrn = _headerInfo.Ukprn,
                    RequestId = req.RequestId,
                    Uln = req.Uln,
                    StandardCode = req.StandardCode,
                    StandardReference = req.StandardReference,
                    FamilyName = req.FamilyName,
                    CertificateReference = req.CertificateReference
                });

            var results = await _apiClient.SubmitCertificates(submitRequest);

            return Ok(results);
        }

        [HttpDelete("{uln}/{familyName}/{standard}/{*certificateReference}")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "The specified Certificate has been deleted.")]
        [SwaggerResponseExample((int)HttpStatusCode.Forbidden, typeof(SwaggerHelpers.Examples.ApiResponseExample))]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, "There are validation errors preventing you from deleting the Certificate.", typeof(ApiResponse))]
        [SwaggerOperation("Delete Certificate", "Deletes the specified Certificate.", Produces = new string[] { "application/json" })]
        public async Task<IActionResult> DeleteCertificate(long uln, string familyName, [SwaggerParameter("Standard Code or Standard Reference Number")] string standard, string certificateReference)
        {
            var deleteRequest = new DeleteBatchCertificateRequest { UkPrn = _headerInfo.Ukprn, Uln = uln, FamilyName = familyName, Standard = standard, CertificateReference = certificateReference};
            var error = await _apiClient.DeleteCertificate(deleteRequest);

            if (error is null)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(error.StatusCode, error);
            }
        }

        [HttpGet("grades")]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(SwaggerHelpers.Examples.GetGradesResponseExample))]
        [SwaggerResponse((int)HttpStatusCode.OK, "The list of valid pass grades.", typeof(string[]))]
        [SwaggerOperation("Get Grades", "To get the list of valid pass grades, to use when creating certificates.", Produces = new string[] { "application/json" })]
        public IActionResult GetGrades()
        {
            var grades = new string[] { CertificateGrade.Pass, CertificateGrade.Credit, CertificateGrade.Merit, CertificateGrade.Distinction, CertificateGrade.PassWithExcellence, CertificateGrade.Outstanding, CertificateGrade.NoGradeAwarded };

            return Ok(grades);
        }
    }
}