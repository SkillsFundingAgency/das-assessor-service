using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Api.Validators;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize]
    [ValidateBadRequest]
    [Route("api/v1/certificates/batch/")]
    public class CertificateBatchController : Controller
    {
        private readonly IMediator _mediator;
        private readonly BatchCertificateRequestValidator _validator;

        public CertificateBatchController(IMediator mediator, BatchCertificateRequestValidator validator)
        {
            _mediator = mediator;
            _validator = validator;
        }

        [HttpPut(Name = "Create")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<BatchCertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Create([FromBody] IEnumerable<BatchCertificateRequest> batchRequest)
        {
            List<BatchCertificateResponse> response = new List<BatchCertificateResponse>();

            foreach (BatchCertificateRequest request in batchRequest)
            {
                ValidationResult validationResult = _validator.Validate(request);

                BatchCertificateResponse certResponse = new BatchCertificateResponse
                {
                    Uln = request.Uln,
                    StandardCode = request.StandardCode,
                    FamilyName = request.FamilyName,
                    ProvidedCertificateData = request.CertificateData,
                    ValidationErrors = validationResult.Errors.Select(error => error.ErrorMessage).ToList()
                };

                if (validationResult.IsValid)
                {
                    // NOTE: We may want to use StartCertificate & UpdateCertificate handlers so that we don't violate DRY
                    certResponse.Certificate = await _mediator.Send(request);
                }

                response.Add(certResponse);
            }

            return Ok(response);

        }
    }
}
