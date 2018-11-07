﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Domain.Entities;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Controllers.Staff
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/staffcertificatereprint")]
    [ValidateBadRequest]
    public class StaffReprintController : Controller
    {
        private readonly IMediator _mediator;
        

        public StaffReprintController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost(Name= "StaffCertificateReprint")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Certificate))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> StaffCertificateReprint([FromBody] StaffCertificateDuplicateRequest staffCertificateDuplicateRequest)
        {
            var result =  await _mediator.Send(staffCertificateDuplicateRequest);
            return Ok(result);

        }
    }
}