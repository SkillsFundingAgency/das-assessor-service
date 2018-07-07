using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Domain.Entities;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize]
    [Route("api/v1/certificates")]
    [ValidateBadRequest]
    public class CertificateQueryController : Controller
    {
        private readonly IMediator _mediator;

        public CertificateQueryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}", Name = "GetCertificate")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Certificate))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetCertificate(Guid id)
        {
            return Ok(await _mediator.Send(new GetCertificateRequest(id)));
        }

        [HttpGet(Name = "GetCertificates")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<CertificateResponse>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetCertificates([FromQuery] List<string> statuses)
        {
            return Ok(await _mediator.Send(new GetCertificatesRequest { Statuses = statuses }));
        }

        [HttpGet("addresses", Name = "GetPreviousAddresses")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<CertificateAddress>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetPreviousAddresses([FromQuery] string userName)
        {
            var addresses = await _mediator.Send(new GetPreviousAddressesRequest { Username = userName });
            if (addresses.Count == 0)
                throw new ResourceNotFoundException();
            return Ok(addresses);
        }

        [HttpGet("contact/previousaddress", Name = "GetContactPreviousAddress")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(CertificateAddress))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetContactPreviousAddress([FromQuery] string username)
        {
            var address = await _mediator.Send(new GetContactPreviousAddressesRequest { Username = username });
            if (address == null)
                throw new ResourceNotFoundException();
            return Ok(address);
        }
    }
}