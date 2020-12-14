using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.BatchLogs;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/batches")]
    [ValidateBadRequest]
    public class BatchLogQueryController : Controller
    {
        private readonly IMediator _mediator;

        public BatchLogQueryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{batchNumber}", Name = "GetBatchLog")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(BatchLogResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetBatchLog(int batchNumber)
        {
            var batchLog = await _mediator.Send(new GetBatchLogRequest {BatchNumber = batchNumber});
            if (batchLog == null)
            {
                return NoContent();
            }

            return Ok(batchLog);
        }

        [HttpGet("batch-number-ready-to-print", Name = "GetBatchNumberReadyToPrint")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(int?))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetBatchNumberReadyToPrint()
        {
            return Ok(await _mediator.Send(new GetBatchNumberReadyToPrintRequest()));
        }
    }
}