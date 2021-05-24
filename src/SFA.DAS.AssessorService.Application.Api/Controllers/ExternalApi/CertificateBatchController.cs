using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Extensions;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NotFound = SFA.DAS.AssessorService.Domain.Exceptions.NotFound;

namespace SFA.DAS.AssessorService.Application.Api.Controllers.ExternalApi
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [ValidateBadRequest]
    [Route("api/v1/certificates/batch/")]
    public class CertificateBatchController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IValidator<GetBatchCertificateRequest> _getValidator;
        private readonly IValidator<CreateBatchCertificateRequest> _createValidator;
        private readonly IValidator<UpdateBatchCertificateRequest> _updateValidator;
        private readonly IValidator<SubmitBatchCertificateRequest> _submitValidator;
        private readonly IValidator<DeleteBatchCertificateRequest> _deleteValidator;

        public CertificateBatchController(IMediator mediator, IValidator<GetBatchCertificateRequest> getValidator, IValidator<CreateBatchCertificateRequest> createValidator, IValidator<UpdateBatchCertificateRequest> updateValidator, IValidator<SubmitBatchCertificateRequest> submitValidator, IValidator<DeleteBatchCertificateRequest> deleteValidator)
        {
            _mediator = mediator;
            _getValidator = getValidator;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _submitValidator = submitValidator;
            _deleteValidator = deleteValidator;
        }

        [HttpGet("{uln}/{lastname}/{standard}/{ukPrn}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GetBatchCertificateResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Get(long uln, string lastname, string standardId, int ukPrn)
        {
            var request = new GetBatchCertificateRequest
            {
                Uln = uln,
                FamilyName = lastname,
                UkPrn = ukPrn
            };

            var standard = await _mediator.Send(new GetStandardVersionRequest { StandardId = standardId });
            
            if (standard != null)
            {
                request.StandardCode = standard.LarsCode;
                request.StandardReference = standard.IfateReferenceNumber;
            }

            var validationResult = await _getValidator.ValidateAsync(request);
            var isRequestValid = validationResult.IsValid;
            var validationErrors = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
            
            GetBatchCertificateResponse getResponse = new GetBatchCertificateResponse
            {
                Uln = request.Uln,
                Standard = standardId,
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
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Create([FromBody] IEnumerable<CreateBatchCertificateRequest> batchRequest)
        {
            var bag = new List<BatchCertificateResponse>();

            foreach (var request in batchRequest)
            {
                Standard standard = null;
                if (!string.IsNullOrEmpty(request?.CertificateData?.Version))
                {
                    standard = await _mediator.Send(
                        new GetStandardVersionRequest { StandardId = request.GetStandardId(), Version = request.CertificateData.Version });
                }
                else
                {
                    standard = await _mediator.Send(new GetCalculatedStandardVersionForApprenticeshipRequest { StandardId = request.GetStandardId(), Uln = request.Uln });
                }

                if (standard != null)
                {
                    request.PopulateMissingFields(standard);
                }

                var validationResult = await _createValidator.ValidateAsync(request);
                var isRequestValid = validationResult.IsValid;
                var validationErrors = validationResult.Errors.Select(error => error.ErrorMessage).ToList();

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
                    certResponse.Certificate = await _mediator.Send(request, new CancellationToken());
                }

                bag.Add(certResponse);
            }

            return Ok(bag.ToList());
        }

        [HttpPut]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<BatchCertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Update([FromBody] IEnumerable<UpdateBatchCertificateRequest> batchRequest)
        {
            var bag = new List<BatchCertificateResponse>();

            foreach (var request in batchRequest)
            {
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
                }

                var validationResult = await _updateValidator.ValidateAsync(request, new CancellationToken());
                var isRequestValid = validationResult.IsValid;
                var validationErrors = validationResult.Errors.Select(error => error.ErrorMessage).ToList();

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

                bag.Add(certResponse);
            }

            return Ok(bag.ToList());
        }

        [HttpPost("submit")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<SubmitBatchCertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Submit([FromBody] IEnumerable<SubmitBatchCertificateRequest> batchRequest)
        {
            var bag = new List<SubmitBatchCertificateResponse>();

            foreach (var request in batchRequest)
            {
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
                }

                var validationResult = await _submitValidator.ValidateAsync(request);
                var isRequestValid = validationResult.IsValid;
                var validationErrors = validationResult.Errors.Select(error => error.ErrorMessage).ToList();

                SubmitBatchCertificateResponse submitResponse = new SubmitBatchCertificateResponse
                {
                    RequestId = request.RequestId,
                    ValidationErrors = validationErrors
                };

                if (!validationErrors.Any() && isRequestValid)
                {
                    submitResponse.Certificate = await _mediator.Send(request);
                }

                bag.Add(submitResponse);
            }

            return Ok(bag.ToList());
        }

        [HttpDelete("{uln}/{lastname}/{standard}/{certificateReference}/{ukPrn}")]
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Delete(long uln, string lastname, string standard, string certificateReference, int ukPrn)
        {
            var request = new DeleteBatchCertificateRequest
            {
                Uln = uln,
                FamilyName = lastname,
                CertificateReference = certificateReference,
                UkPrn = ukPrn
            };

            var collatedStandard = int.TryParse(standard, out int standardCode) ? await GetCollatedStandard(standardCode) : await GetCollatedStandard(standard);

            if (collatedStandard != null)
            {
                request.StandardCode = collatedStandard.StandardId ?? int.MinValue;
                request.StandardReference = collatedStandard.ReferenceNumber;
            }

            var validationResult = await _deleteValidator.ValidateAsync(request);
            var isRequestValid = validationResult.IsValid;
            var validationErrors = validationResult.Errors.Select(error => error.ErrorMessage).ToList();

            if (!validationErrors.Any() && isRequestValid)
            {
                try
                {
                    await _mediator.Send(request);
                    return NoContent();
                }
                catch (NotFound)
                {
                    return NotFound();
                }
            }
            else
            {
                ApiResponse response = new ApiResponse((int)HttpStatusCode.Forbidden, string.Join("; ", validationErrors));
                return StatusCode(response.StatusCode, response);
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
