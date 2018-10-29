using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Domain.Entities;
using Swashbuckle.AspNetCore.SwaggerGen;
using NotFound = SFA.DAS.AssessorService.Domain.Exceptions.NotFound;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [ValidateBadRequest]
    [Route("api/v1/certificates/")]
    public class CertificateController : Controller
    {
        private readonly IMediator _mediator;

        public CertificateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("start", Name = "Start")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Certificate))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Start([FromBody] StartCertificateRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpPost("startprivate", Name = "StartPrivate")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Certificate))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> StartPrivate([FromBody] StartCertificatePrivateRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpPut("update", Name = "Update")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Certificate))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Update([FromBody] UpdateCertificateRequest certificate)
        {
            return Ok(await _mediator.Send(certificate));
        }

        [HttpPut("{batchNumber}", Name = "UpdateCertificatesBatchToIndicatePrinted")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Certificate))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateCertificatesBatchToIndicatePrinted(int batchNumber, [FromBody] UpdateCertificatesBatchToIndicatePrintedRequest updateCertificatesBatchToIndicatePrintedRequest)
        {
            await _mediator.Send(updateCertificatesBatchToIndicatePrintedRequest);
            return Ok();
        }

        [HttpPost("requestreprint", Name = "RequestReprint")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Certificate))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> RequestReprint([FromBody] CertificateReprintRequest certificateReprintRequest)
        {
            try
            {
                await _mediator.Send(certificateReprintRequest);
            }
            catch (NotFound)
            {
                throw new ResourceNotFoundException();
            }

            return Ok();
        }

        [HttpPut("updatestatustobeapproved", Name = "UpdatePrivateCertificationCertificateStatusToBeApproved")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdatePrivateCertificationCertificateStatusToBeApproved([FromBody] UpdateCertificateRequestToBeApproved certificate)
        {
            await _mediator.Send(certificate);
            return Ok();
        }

        [HttpPost("approvals", Name = "Approvals")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Certificate))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Approvals([FromBody] CertificateApprovalRequest certificateApprovalRequest)
        {
            try
            {
                await _mediator.Send(certificateApprovalRequest);
            }
            catch (NotFound)
            {
                throw new ResourceNotFoundException();
            }

            return Ok();
        }

        [HttpPost("prepareprivatecertificatesforbatchrun", Name = "PreparePrivateCertificatesforBatchRun")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Certificate))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> PreparePrivateCertificatesforBatchRun([FromBody] PrivateCertificatePrepareForBatchRunRequest privateCertificatePrepareForBatchRunRequest)
        {
            await _mediator.Send(privateCertificatePrepareForBatchRunRequest);
            return Ok();
        }
    }
}