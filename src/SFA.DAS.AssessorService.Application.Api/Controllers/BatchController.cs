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
using SFA.DAS.AssessorService.Domain.Entities;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize]
    [ValidateBadRequest]
    [Route("api/v1/batches/")]
    public class BatchController : Controller
    {
        private readonly IMediator _mediator;

        public BatchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost(Name = "CreateBatchLog")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(BatchLogResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CreateBatchLog([FromBody] CreateBatchLogRequest request)
        {
            return Ok(await _mediator.Send(request));
        }
    }
} 