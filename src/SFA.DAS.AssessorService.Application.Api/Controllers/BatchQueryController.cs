using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Domain.Entities;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize]
    [Route("api/v1/batches")]
    [ValidateBadRequest]
    public class BatchQueryController : Controller
    {
        private readonly IMediator _mediator;

        public BatchQueryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet(Name = "GetLastBatchLog")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(BatchLogResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetLastBatchLog()
        {
            var batchLog = await _mediator.Send(new GetLastBatchLogRequest());
            if (batchLog == null)
                return NoContent();
            return Ok(batchLog);
        }
    }
}