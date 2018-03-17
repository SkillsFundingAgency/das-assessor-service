using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize]
    [Route("api/v1/certificate")]
    public class CertificateController : Controller
    {
        private readonly IMediator _mediator;

        public CertificateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost(Name = "Start")]
        public async Task<IActionResult> Start([FromBody] StartCertificateRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpGet("{id}", Name = "GetCertificate")]
        public async Task<IActionResult> GetCertificate(Guid id)
        {
            return Ok(await _mediator.Send(new GetCertificateRequest(id)));
        }

        [HttpPut(Name = "Update")]
        public async Task<IActionResult> Update(Certificate certificate)
        {
            return Ok(await _mediator.Send(new UpdateCertificateRequest(certificate)));
        }
    }
}