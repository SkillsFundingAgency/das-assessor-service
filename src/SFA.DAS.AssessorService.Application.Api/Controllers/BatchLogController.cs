using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [ValidateBadRequest]
    [Route("api/v1/batches/")]
    public class BatchLogController : Controller
    {
        private readonly IMediator _mediator;

        public BatchLogController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create", Name = "CreateBatchLog")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(BatchLogResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Create([FromBody] CreateBatchLogRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpPut("sent-to-printer", Name = "SentToPrinter")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ValidationResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SentToPrinter([FromBody] SentToPrinterBatchLogRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpPut("printed", Name = "Printed")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ValidationResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Printed([FromBody] PrintedBatchLogRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpPut("update-batch-data",Name= "UpdateBatchData")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ValidationResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateBatchData([FromBody] UpdateBatchDataBatchLogRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

    }
} 