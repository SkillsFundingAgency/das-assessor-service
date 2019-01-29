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
using SFA.DAS.AssessorService.Domain.JsonData.Printing;
using Swashbuckle.AspNetCore.SwaggerGen;

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

        [HttpGet("latest", Name = "GetLastBatchLog")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(BatchLogResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetLastBatchLog()
        {
            var batchLog = await _mediator.Send(new GetLastBatchLogRequest());
            if (batchLog.CertificatesFileName == null)
                return NoContent();
            return Ok(batchLog);
        }

        [HttpGet("{batchNumber}", Name = "GetBatchLogForBatchNumber")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(BatchLogResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetBatchLogForPeriodAndBatchNumber(string batchNumber)
        {
            var batchLog = await _mediator.Send(new GetBatchFromBatchNumberRequest {BatchNumber = batchNumber});
            if (batchLog == null)
                return NoContent();
            return Ok(batchLog);
        }
    }
}