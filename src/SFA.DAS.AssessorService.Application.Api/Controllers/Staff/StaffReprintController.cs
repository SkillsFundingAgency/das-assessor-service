using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Domain.Paging;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers.Staff
{
    [Authorize]
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
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(StaffUIReprintResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> StaffCertificateReprint([FromBody] StaffUIReprintRequest staffUiReprintRequest)
        {
            return Ok(await _mediator.Send(staffUiReprintRequest));
        }
    }
}