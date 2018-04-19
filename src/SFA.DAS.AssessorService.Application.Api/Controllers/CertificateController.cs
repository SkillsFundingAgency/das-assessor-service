using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Domain.Entities;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize]
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

        [HttpPut("update", Name = "Update")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Certificate))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Update([FromBody] UpdateCertificateRequest certificate)
        {
            return Ok(await _mediator.Send(certificate));
        }

        [HttpPut("{batchNumber}", Name = "UpdateStatus")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Certificate))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateStatus(string batchNumber, [FromBody] UpdateCertificateStatusRequest updateCertificateStatusRequest)
        {
            await _mediator.Send(updateCertificateStatusRequest);
            return Ok();
        }
    }
} 