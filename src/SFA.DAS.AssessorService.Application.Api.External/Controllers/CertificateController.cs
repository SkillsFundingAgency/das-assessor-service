using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.External.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.External.Messages;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using SFA.DAS.AssessorService.Application.Api.External.Models.Certificates;
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
        private const string CERTIFICATE_STATUS_DRAFT = "Draft";
        private const string CERTIFICATE_STATUS_READY = "Ready";
        private const string CERTIFICATE_STATUS_SUBMITTED = "Submitted";
        private const string CERTIFICATE_STATUS_PRINTED = "Printed";
        private const string CERTIFICATE_STATUS_REPRINT = "Reprint";

        private readonly ILogger<CertificateController> _logger;
        private readonly IHeaderInfo _headerInfo;
        private readonly IApiClient _apiClient;

        public CertificateController(ILogger<CertificateController> logger, IHeaderInfo headerInfo, IApiClient apiClient)
        {
            _logger = logger;
            _headerInfo = headerInfo;
            _apiClient = apiClient;
        }

        [HttpGet("{uln}/{familyName}/{standardCode}")]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(SwaggerHelpers.Examples.CertificateExample))]
        [SwaggerResponse((int)HttpStatusCode.OK, "The current Certificate.", typeof(Certificate))]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "There is no Certificate and you may create one.")]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, "There are validation errors preventing you from retrieving the Certificate.", typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerOperation("Get Certificate", "Gets the specified Certificate.")]
        public async Task<IActionResult> GetCertificate(long uln, string familyName, int standardCode)
        {
            GetCertificateRequest getRequest = new GetCertificateRequest { UkPrn = _headerInfo.Ukprn, Email = _headerInfo.Email, Uln = uln, FamilyName = familyName, StandardCode = standardCode };
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
            else
            {
                if(IsDraftCertificateDeemedAsReady(response.Certificate))
                {
                    response.Certificate.Status.CurrentStatus = CERTIFICATE_STATUS_READY;
                }
                else if (response.Certificate.Status.CurrentStatus == CERTIFICATE_STATUS_PRINTED || response.Certificate.Status.CurrentStatus == CERTIFICATE_STATUS_REPRINT)
                {
                    response.Certificate.Status.CurrentStatus = CERTIFICATE_STATUS_SUBMITTED;
                }

                return Ok(response.Certificate);
            }
        }

        [HttpPost]
        [SwaggerRequestExample(typeof(IEnumerable<CreateCertificate>), typeof(SwaggerHelpers.Examples.CreateCertificateExample))]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(SwaggerHelpers.Examples.BatchCertificateResponseExample))]
        [SwaggerResponse((int)HttpStatusCode.OK, "For each item: The created Certificate if valid, else a list of validation errors.", typeof(IEnumerable<BatchCertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerOperation("Create Certificates", "Creates a new Certificate for each valid item within the request.")]
        public async Task<IActionResult> CreateCertificates([FromBody] IEnumerable<CreateCertificate> request)
        {
            IEnumerable<BatchCertificateRequest> bcRequest = request.Select(req => 
                new BatchCertificateRequest {
                    UkPrn = _headerInfo.Ukprn,
                    Email = _headerInfo.Email,
                    RequestId = req.RequestId,
                    CertificateData = new CertificateData
                        {
                        Standard = req.Standard,
                        Learner = req.Learner,
                        LearningDetails = req.LearningDetails,
                        PostalContact = req.PostalContact
                    }
                });

            var results = await _apiClient.CreateCertificates(bcRequest);

            foreach(var result in results)
            {
                if (IsDraftCertificateDeemedAsReady(result.Certificate))
                {
                    result.Certificate.Status.CurrentStatus = CERTIFICATE_STATUS_READY;
                }
            }

            return Ok(results);
        }

        [HttpPut]
        [SwaggerRequestExample(typeof(IEnumerable<UpdateCertificate>), typeof(SwaggerHelpers.Examples.UpdateCertificateExample))]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(SwaggerHelpers.Examples.BatchCertificateResponseExample))]
        [SwaggerResponse((int)HttpStatusCode.OK, "For each item: The updated Certificate if valid, else a list of validation errors.", typeof(IEnumerable<BatchCertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerOperation("Update Certificates", "Updates the specified Certificate with the information contained in each valid request.")]
        public async Task<IActionResult> UpdateCertificates([FromBody] IEnumerable<UpdateCertificate> request)
        {
            IEnumerable<BatchCertificateRequest> bcRequest = request.Select(req =>
                new BatchCertificateRequest
                {
                    UkPrn = _headerInfo.Ukprn,
                    Email = _headerInfo.Email,
                    RequestId = req.RequestId,
                    CertificateData = new CertificateData
                    {
                        CertificateReference = req.CertificateReference,
                        Standard = req.Standard,
                        Learner = req.Learner,
                        LearningDetails = req.LearningDetails,
                        PostalContact = req.PostalContact
                    }
                });

            var results = await _apiClient.UpdateCertificates(bcRequest);

            foreach (var result in results)
            {
                if (IsDraftCertificateDeemedAsReady(result.Certificate))
                {
                    result.Certificate.Status.CurrentStatus = CERTIFICATE_STATUS_READY;
                }
            }

            return Ok(results);
        }

        [HttpPost("submit")]
        [SwaggerRequestExample(typeof(IEnumerable<SubmitCertificate>), typeof(SwaggerHelpers.Examples.SubmitCertificateExample))]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(SwaggerHelpers.Examples.SubmitBatchCertificateResponseExample))]
        [SwaggerResponse((int)HttpStatusCode.OK, "For each item: The submitted Certificate if valid, else a list of validation errors.", typeof(IEnumerable<SubmitBatchCertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerOperation("Submit Certificates", "Submits the specified Certificate for each valid request.")]
        public async Task<IActionResult> SubmitCertificates([FromBody] IEnumerable<SubmitCertificate> request)
        {
            IEnumerable<SubmitBatchCertificateRequest> scRequest = request.Select(req => 
                new SubmitBatchCertificateRequest
                {
                    UkPrn = _headerInfo.Ukprn,
                    Email = _headerInfo.Email,
                    RequestId = req.RequestId, Uln = req.Uln,
                    StandardCode = req.StandardCode, FamilyName = req.FamilyName,
                    CertificateReference = req.CertificateReference
                });

            var results = await _apiClient.SubmitCertificates(scRequest);

            return Ok(results);
        }

        [HttpDelete("{uln}/{familyName}/{standardCode}/{certificateReference}")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "The specified Certificate has been deleted.")]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, "There are validation errors preventing you from deleting the Certificate.", typeof(ApiResponse))]
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

        #region Utility Functions
        private bool IsDraftCertificateDeemedAsReady(Certificate certificate)
        {
            // Note: This for the External API only and allows the caller to know if a Draft Certificate is 'Ready' for submitting
            // It is deemed ready if the mandatory fields have been filled out.
            if (certificate?.CertificateData is null || certificate?.Status?.CurrentStatus != CERTIFICATE_STATUS_DRAFT || string.IsNullOrEmpty(certificate.CertificateData.CertificateReference))
            {
                return false;
            }
            else if (certificate.CertificateData.Standard is null || certificate.CertificateData.Standard.StandardCode < 1)
            {
                return false;
            }
            else if (certificate.CertificateData.PostalContact is null || string.IsNullOrEmpty(certificate.CertificateData.PostalContact.ContactName)
                        || string.IsNullOrEmpty(certificate.CertificateData.PostalContact.Organisation) || string.IsNullOrEmpty(certificate.CertificateData.PostalContact.City)
                        || string.IsNullOrEmpty(certificate.CertificateData.PostalContact.PostCode))
            {
                return false;
            }
            else if (certificate.CertificateData.Learner is null || string.IsNullOrEmpty(certificate.CertificateData.Learner.FamilyName)
                        || certificate.CertificateData.Learner.Uln < 1000000000 || certificate.CertificateData.Learner.Uln > 9999999999)
            {
                return false;
            }
            else if (certificate.CertificateData.LearningDetails is null || string.IsNullOrEmpty(certificate.CertificateData.LearningDetails.OverallGrade)
                        || !certificate.CertificateData.LearningDetails.AchievementDate.HasValue)
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}