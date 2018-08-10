using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Api.Validators.Certificates;
using SFA.DAS.AssessorService.Application.Exceptions;
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
        private readonly IMediator _mediator;
        private readonly CreateBatchCertificateRequestValidator _createValidator;
        private readonly UpdateBatchCertificateRequestValidator _updateValidator;
        private readonly SubmitBatchCertificateRequestValidator _submitValidator;
        private readonly DeleteBatchCertificateRequestValidator _deleteValidator;

        public CertificateBatchController(IMediator mediator, CreateBatchCertificateRequestValidator createValidator, UpdateBatchCertificateRequestValidator updateValidator, SubmitBatchCertificateRequestValidator submitValidator, DeleteBatchCertificateRequestValidator deleteValidator)
        {
            _mediator = mediator;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _submitValidator = submitValidator;
            _deleteValidator = deleteValidator;
        }

        [HttpPut]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<BatchCertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Create([FromBody] IEnumerable<CreateBatchCertificateRequest> batchRequest)
        {
            List<BatchCertificateResponse> response = new List<BatchCertificateResponse>();

            foreach (CreateBatchCertificateRequest request in batchRequest)
            {
                ValidationResult validationResult = _createValidator.Validate(request);

                BatchCertificateResponse certResponse = new BatchCertificateResponse
                {
                    Uln = request.Uln,
                    StandardCode = request.StandardCode,
                    FamilyName = request.FamilyName,
                    ProvidedCertificateReference = request.CertificateReference,
                    ProvidedCertificateData = request.CertificateData,
                    ValidationErrors = validationResult.Errors.Select(error => error.ErrorMessage).ToList()
                };

                if (validationResult.IsValid)
                {
                    certResponse.Certificate = await _mediator.Send(request);
                }

                response.Add(certResponse);
            }

            return Ok(response);

        }

        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<BatchCertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Update([FromBody] IEnumerable<UpdateBatchCertificateRequest> batchRequest)
        {
            List<BatchCertificateResponse> response = new List<BatchCertificateResponse>();

            foreach (UpdateBatchCertificateRequest request in batchRequest)
            {
                ValidationResult validationResult = _updateValidator.Validate(request);

                BatchCertificateResponse certResponse = new BatchCertificateResponse
                {
                    Uln = request.Uln,
                    StandardCode = request.StandardCode,
                    FamilyName = request.FamilyName,
                    ProvidedCertificateReference = request.CertificateReference,
                    ProvidedCertificateData = request.CertificateData,
                    ValidationErrors = validationResult.Errors.Select(error => error.ErrorMessage).ToList()
                };

                if (validationResult.IsValid)
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
                ValidationResult validationResult = _submitValidator.Validate(request);

                SubmitBatchCertificateResponse submitResponse = new SubmitBatchCertificateResponse
                {
                    Uln = request.Uln,
                    StandardCode = request.StandardCode,
                    FamilyName = request.FamilyName,
                    ValidationErrors = validationResult.Errors.Select(error => error.ErrorMessage).ToList()
                };

                if (validationResult.IsValid)
                {
                    submitResponse.Certificate = await _mediator.Send(request);
                }

                response.Add(submitResponse);
            }

            return Ok(response);
        }

        [HttpDelete("{uln}/{lastname}/{standardcode}/{ukPrn}/{username}")]
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Delete(long uln, string lastname, int standardcode, int ukPrn, string username)
        {
            var request = new DeleteBatchCertificateRequest
            {
                Uln = uln,
                FamilyName = lastname,
                StandardCode = standardcode,
                UkPrn = ukPrn,
                Username = username
            };

            ValidationResult validationResult = _deleteValidator.Validate(request);

            if (validationResult.IsValid)
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
                ApiResponse response = new ApiResponse((int)HttpStatusCode.BadRequest, validationResult.Errors.ToString());
                return BadRequest(response);
            }
        }
    }
}
