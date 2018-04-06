﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Attributes;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Domain.Entities;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize]
    [Route("api/v1/certificate")]
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

        [HttpGet(Name = "GetCertificatesToBePrinted")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<Certificate>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetCertificatesToBePrinted()
        {
            return Ok(await _mediator.Send(new GetCertificatesToBePrintedRequest()));
        }
    }
}