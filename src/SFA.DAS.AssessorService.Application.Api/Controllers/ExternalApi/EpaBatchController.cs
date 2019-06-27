using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
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

namespace SFA.DAS.AssessorService.Application.Api.Controllers.ExternalApi
{
    [Authorize]
    [ValidateBadRequest]
    [Route("api/v1/epas/batch/")]
    public class EpaBatchController : Controller
    {
        private readonly IMediator _mediator;
        private readonly CreateBatchCertificateRequestValidator _createValidator;
        private readonly UpdateBatchCertificateRequestValidator _updateValidator;
        private readonly DeleteBatchCertificateRequestValidator _deleteValidator;

        public EpaBatchController(IMediator mediator, CreateBatchCertificateRequestValidator createValidator, UpdateBatchCertificateRequestValidator updateValidator, DeleteBatchCertificateRequestValidator deleteValidator)
        {
            _mediator = mediator;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _deleteValidator = deleteValidator;
        }

        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<BatchCertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Create([FromBody] IEnumerable<CreateBatchCertificateRequest> batchRequest)
        {
            //var bag = new ConcurrentBag<BatchCertificateResponse>();
            var bag = new List<BatchCertificateResponse>();

            foreach (var request in batchRequest)
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
                }

                var validationResult = await _createValidator.ValidateAsync(request);
                isRequestValid = validationResult.IsValid;
                validationErrors = validationResult.Errors.Select(error => error.ErrorMessage).ToList();

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

        [HttpPut]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<BatchCertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Update([FromBody] IEnumerable<UpdateBatchCertificateRequest> batchRequest)
        {
            //var bag = new ConcurrentBag<BatchCertificateResponse>();
            var bag = new List<BatchCertificateResponse>();

            foreach (var request in batchRequest)
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
                }

                var validationResult = await _updateValidator.ValidateAsync(request);
                isRequestValid = validationResult.IsValid;
                validationErrors = validationResult.Errors.Select(error => error.ErrorMessage).ToList();

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

        [HttpDelete("{uln}/{lastname}/{standard}/{epaReference}/{ukPrn}/{*email}")]
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Delete(long uln, string lastname, string standard, string epaReference, int ukPrn, string email)
        {
            var request = new DeleteBatchCertificateRequest
            {
                Uln = uln,
                FamilyName = lastname,
                CertificateReference = epaReference,
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
            }

            var validationResult = await _deleteValidator.ValidateAsync(request);
            isRequestValid = validationResult.IsValid;
            validationErrors = validationResult.Errors.Select(error => error.ErrorMessage).ToList();

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