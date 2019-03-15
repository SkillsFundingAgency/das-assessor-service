using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Api.Validators.Certificates;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NotFound = SFA.DAS.AssessorService.Domain.Exceptions.NotFound;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize]
    [ValidateBadRequest]
    [Route("api/v1/certificates/batch/")]
    public class CertificateBatchController : Controller
    {
        private const string INVALID_STANDARD_SPECIFIED = "Unable to find specified Standard";

        private readonly IMediator _mediator;
        private readonly GetBatchCertificateRequestValidator _getValidator;
        private readonly CreateBatchCertificateRequestValidator _createValidator;
        private readonly UpdateBatchCertificateRequestValidator _updateValidator;
        private readonly SubmitBatchCertificateRequestValidator _submitValidator;
        private readonly DeleteBatchCertificateRequestValidator _deleteValidator;

        public CertificateBatchController(IMediator mediator, GetBatchCertificateRequestValidator getValidator, CreateBatchCertificateRequestValidator createValidator, UpdateBatchCertificateRequestValidator updateValidator, SubmitBatchCertificateRequestValidator submitValidator, DeleteBatchCertificateRequestValidator deleteValidator)
        {
            _mediator = mediator;
            _getValidator = getValidator;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _submitValidator = submitValidator;
            _deleteValidator = deleteValidator;
        }

        [HttpGet("{uln}/{lastname}/{standard}/{ukPrn}/{*email}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GetBatchCertificateResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Get(long uln, string lastname, string standard, int ukPrn, string email)
        {
            var request = new GetBatchCertificateRequest
            {
                Uln = uln,
                FamilyName = lastname,
                UkPrn = ukPrn,
                Email = email
            };

            var validationErrors = new List<string>();
            var isRequestValid = false;

            var collatedStandard = int.TryParse(standard, out int standardCode) ? await GetCollatedStandard(standardCode) : await GetCollatedStandard(standard);

            if (collatedStandard != null)
            {
                request.StandardCode = collatedStandard.StandardId ?? int.MinValue;
                request.StandardReference = collatedStandard.ReferenceNumber;

                ValidationResult validationResult = _getValidator.Validate(request);
                isRequestValid = validationResult.IsValid;
                validationErrors = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
            }
            else
            {
                validationErrors.Add(INVALID_STANDARD_SPECIFIED);
            }

            GetBatchCertificateResponse getResponse = new GetBatchCertificateResponse
            {
                Uln = request.Uln,
                Standard = standard,
                FamilyName = request.FamilyName,
                ValidationErrors = validationErrors
            };

            if (!validationErrors.Any() && isRequestValid)
            {
                getResponse.Certificate = await _mediator.Send(request);
            }

            return Ok(getResponse);
        }

        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<BatchCertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Create([FromBody] IEnumerable<CreateBatchCertificateRequest> batchRequest)
        {
            List<BatchCertificateResponse> response = new List<BatchCertificateResponse>();

            foreach (CreateBatchCertificateRequest request in batchRequest)
            {
                var validationErrors = new List<string>();
                var isRequestValid = false;

                var collatedStandard = request.StandardCode > 0 ? await GetCollatedStandard(request.StandardCode) : await GetCollatedStandard(request.StandardReference);

                if(collatedStandard != null)
                {
                    // Only fill in the missing bits...
                    if (request.StandardCode < 1)
                    {
                        request.StandardCode = collatedStandard.StandardId ?? int.MinValue;
                    }
                    else if (string.IsNullOrEmpty(request.StandardReference))
                    {
                        request.StandardReference = collatedStandard.ReferenceNumber;
                    }

                    ValidationResult validationResult = _createValidator.Validate(request);
                    isRequestValid = validationResult.IsValid;
                    validationErrors = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
                }
                else
                {
                    validationErrors.Add(INVALID_STANDARD_SPECIFIED);
                }
                
                BatchCertificateResponse certResponse = new BatchCertificateResponse
                {
                    RequestId = request.RequestId,
                    Uln = request.Uln,
                    StandardCode = request.StandardCode,
                    StandardReference = request.StandardReference,
                    FamilyName = request.FamilyName,
                    ValidationErrors = validationErrors
                };

                if (!validationErrors.Any() && isRequestValid)
                {
                    certResponse.Certificate = await _mediator.Send(request);
                }

                response.Add(certResponse);
            }

            return Ok(response);

        }

        [HttpPut]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<BatchCertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Update([FromBody] IEnumerable<UpdateBatchCertificateRequest> batchRequest)
        {
            List<BatchCertificateResponse> response = new List<BatchCertificateResponse>();

            foreach (UpdateBatchCertificateRequest request in batchRequest)
            {
                var validationErrors = new List<string>();
                var isRequestValid = false;

                var collatedStandard = request.StandardCode > 0 ? await GetCollatedStandard(request.StandardCode) : await GetCollatedStandard(request.StandardReference);

                if (collatedStandard != null)
                {
                    // Only fill in the missing bits...
                    if (request.StandardCode < 1)
                    {
                        request.StandardCode = collatedStandard.StandardId ?? int.MinValue;
                    }
                    else if (string.IsNullOrEmpty(request.StandardReference))
                    {
                        request.StandardReference = collatedStandard.ReferenceNumber;
                    }

                    ValidationResult validationResult = _updateValidator.Validate(request);
                    isRequestValid = validationResult.IsValid;
                    validationErrors = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
                }
                else
                {
                    validationErrors.Add(INVALID_STANDARD_SPECIFIED);
                }

                BatchCertificateResponse certResponse = new BatchCertificateResponse
                {
                    RequestId = request.RequestId,
                    Uln = request.Uln,
                    StandardCode = request.StandardCode,
                    StandardReference = request.StandardReference,
                    FamilyName = request.FamilyName,
                    ValidationErrors = validationErrors
                };

                if (!validationErrors.Any() && isRequestValid)
                {
                    certResponse.Certificate = await _mediator.Send(request);
                }

                response.Add(certResponse);
            }

            return Ok(response);

        }

        [HttpPost("submit")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<SubmitBatchCertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Submit([FromBody] IEnumerable<SubmitBatchCertificateRequest> batchRequest)
        {
            List<SubmitBatchCertificateResponse> response = new List<SubmitBatchCertificateResponse>();

            foreach (SubmitBatchCertificateRequest request in batchRequest)
            {
                var validationErrors = new List<string>();
                var isRequestValid = false;

                var collatedStandard = request.StandardCode > 0 ? await GetCollatedStandard(request.StandardCode) : await GetCollatedStandard(request.StandardReference);

                if (collatedStandard != null)
                {
                    // Only fill in the missing bits...
                    if (request.StandardCode < 1)
                    {
                        request.StandardCode = collatedStandard.StandardId ?? int.MinValue;
                    }
                    else if (string.IsNullOrEmpty(request.StandardReference))
                    {
                        request.StandardReference = collatedStandard.ReferenceNumber;
                    }

                    ValidationResult validationResult = _submitValidator.Validate(request);
                    isRequestValid = validationResult.IsValid;
                    validationErrors = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
                }
                else
                {
                    validationErrors.Add(INVALID_STANDARD_SPECIFIED);
                }

                SubmitBatchCertificateResponse submitResponse = new SubmitBatchCertificateResponse
                {
                    RequestId = request.RequestId,
                    ValidationErrors = validationErrors
                };

                if (!validationErrors.Any() && isRequestValid)
                {
                    submitResponse.Certificate = await _mediator.Send(request);
                }

                response.Add(submitResponse);
            }

            return Ok(response);
        }

        [HttpDelete("{uln}/{lastname}/{standard}/{certificateReference}/{ukPrn}/{*email}")]
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Delete(long uln, string lastname, string standard, string certificateReference, int ukPrn, string email)
        {
            var request = new DeleteBatchCertificateRequest
            {
                Uln = uln,
                FamilyName = lastname,
                CertificateReference = certificateReference,
                UkPrn = ukPrn,
                Email = email
            };

            var validationErrors = new List<string>();
            var isRequestValid = false;

            var collatedStandard = int.TryParse(standard, out int standardCode) ? await GetCollatedStandard(standardCode) : await GetCollatedStandard(standard);

            if (collatedStandard != null)
            {
                request.StandardCode = collatedStandard.StandardId ?? int.MinValue;
                request.StandardReference = collatedStandard.ReferenceNumber;

                ValidationResult validationResult = _deleteValidator.Validate(request);
                isRequestValid = validationResult.IsValid;
                validationErrors = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
            }
            else
            {
                validationErrors.Add(INVALID_STANDARD_SPECIFIED);
            }

            if (!validationErrors.Any() && isRequestValid)
            {
                try
                {
                    await _mediator.Send(request);
                    return NoContent();
                }
                catch(NotFound)
                {
                    return NotFound();
                }
            }
            else
            {
                ApiResponse response = new ApiResponse((int)HttpStatusCode.Forbidden, string.Join("; ", validationErrors));
                return BadRequest(response);
            }
        }


        private async Task<StandardCollation> GetCollatedStandard(string referenceNumber)
        {
            return await _mediator.Send(new GetCollatedStandardRequest { ReferenceNumber = referenceNumber });
        }

        private async Task<StandardCollation> GetCollatedStandard(int standardId)
        {
            return await _mediator.Send(new GetCollatedStandardRequest { StandardId = standardId });
        }
    }
}
