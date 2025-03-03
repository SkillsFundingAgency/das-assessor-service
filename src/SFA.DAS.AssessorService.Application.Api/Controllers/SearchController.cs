using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/search")]
    [ValidateBadRequest]
    public class SearchController : Controller
    {
        private readonly IMediator _mediator;

        public SearchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost(Name = "Search")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<SearchResult>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Search([FromBody]SearchQuery searchQuery)
        {
            return Ok(await _mediator.Send(searchQuery));
        }

        [HttpPost("frameworks", Name = "SearchFrameworks")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<FrameworkSearchResult>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SearchFrameworks([FromBody]FrameworkSearchQuery frameworkSearchQuery)
        {
            return Ok(await _mediator.Send(frameworkSearchQuery));
        }

        [HttpGet("framework-certificate/{id}", Name = "GetFrameworkCertificate")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GetFrameworkCertificateResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetFrameworkCertificate(Guid id)
        {
            return Ok(await _mediator.Send(new GetFrameworkCertificateQuery { Id = id}));
        }
    }
}