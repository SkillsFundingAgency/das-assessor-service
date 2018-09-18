using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Handlers.Staff;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers.Staff
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/staffinvestigation")]
    [ValidateBadRequest]
    public class StaffInvestigationController : Controller
    {
        private readonly IMediator _mediator;
        

        public StaffInvestigationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(InvestigationResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> InvestigateSearch(string surname, long uln, string epaorgid, string username)
        {
            return Ok(await _mediator.Send(new InvestigateSearchRequest(surname, uln, epaorgid, username)));
        }
    }
}