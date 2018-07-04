using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Handlers.Staff;
using SFA.DAS.AssessorService.Domain.Paging;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers.Staff
{
    [Authorize]
    [Route("api/v1/staffsearch")]
    [ValidateBadRequest]
    public class StaffSearchController : Controller
    {
        private readonly IMediator _mediator;
        

        public StaffSearchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(PaginatedList<StaffSearchResult>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Search(string searchQuery, int page = 0)
        {
            return Ok(await _mediator.Send(new StaffSearchRequest(searchQuery, page)));
        }
    }
}