using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Application.Api.Extensions;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Controllers.ExternalApi
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [ValidateBadRequest]
    [Route("api/v1/epas/batch/")]
    public class EpaBatchController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IValidator<CreateBatchEpaRequest> _createValidator;
        private readonly IValidator<UpdateBatchEpaRequest> _updateValidator;
        private readonly IValidator<DeleteBatchEpaRequest> _deleteValidator;

        public EpaBatchController(IMediator mediator,
            IValidator<CreateBatchEpaRequest> createValidator,
            IValidator<UpdateBatchEpaRequest> updateValidator,
            IValidator<DeleteBatchEpaRequest> deleteValidator)
        {
            _mediator = mediator;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _deleteValidator = deleteValidator;
        }

        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<BatchEpaResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Create([FromBody] IEnumerable<CreateBatchEpaRequest> batchRequest)
        {
            var bag = new List<BatchEpaResponse>();

            foreach (var request in batchRequest)
            {
                var validationErrors = new List<string>();
                var isRequestValid = false;
                Standard standard = null;

                if (!string.IsNullOrEmpty(request.Version))
                {
                    standard = await _mediator.Send(
                        new GetStandardVersionRequest { StandardId = request.GetStandardId(), Version = request.Version });
                }
                else
                {
                    standard = await _mediator.Send(new GetCalculatedStandardVersionForApprenticeshipRequest { StandardId = request.GetStandardId(), Uln = request.Uln });
                }

                request.PopulateMissingFields(standard);

                var validationResult = await _createValidator.ValidateAsync(request);
                isRequestValid = validationResult.IsValid;
                validationErrors = validationResult.Errors.Select(error => error.ErrorMessage).ToList();

                var epaResponse = new BatchEpaResponse
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
                    epaResponse.EpaDetails = await _mediator.Send(request);
                }

                bag.Add(epaResponse);
            }

            return Ok(bag.ToList());
        }

        [HttpPut]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<BatchEpaResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Update([FromBody] IEnumerable<UpdateBatchEpaRequest> batchRequest)
        {
            var bag = new List<BatchEpaResponse>();

            foreach (var request in batchRequest)
            {
                Standard standard = null;
                if (!string.IsNullOrEmpty(request.Version))
                {
                    standard = await _mediator.Send(
                        new GetStandardVersionRequest { StandardId = request.GetStandardId(), Version = request.Version });
                }
                else
                {
                    standard = await _mediator.Send(new GetCalculatedStandardVersionForApprenticeshipRequest { StandardId = request.GetStandardId(), Uln = request.Uln });
                }

                // Get Existing Certificate if it exists
                if (standard != null)
                {
                    var existingCertificate = await _mediator.Send(new GetCertificateForUlnRequest { StandardCode = standard.LarsCode, Uln = request.Uln });
                    request.PopulateMissingFields(standard, existingCertificate);
                }

                var validationResult = await _updateValidator.ValidateAsync(request);
                var isRequestValid = validationResult.IsValid;
                var validationErrors = validationResult.Errors.Select(error => error.ErrorMessage).ToList();

                var epaResponse = new BatchEpaResponse
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
                    epaResponse.EpaDetails = await _mediator.Send(request);
                }

                bag.Add(epaResponse);
            }

            return Ok(bag.ToList());
        }

        [HttpDelete("{uln}/{lastname}/{standard}/{epaReference}/{ukPrn}")]
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Delete(long uln, string lastname, string standard, string epaReference, int ukPrn)
        {
            var request = new DeleteBatchEpaRequest
            {
                Uln = uln,
                FamilyName = lastname,
                EpaReference = epaReference,
                UkPrn = ukPrn
            };

            var standardVersion = await _mediator.Send(new GetStandardVersionRequest { StandardId = standard });
            if (standardVersion != null)
            {
                request.StandardCode = standardVersion.LarsCode;
                request.StandardReference = standardVersion.IfateReferenceNumber;
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
                catch (NotFoundException)
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
    }
}