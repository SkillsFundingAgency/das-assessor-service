using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.BatchLogs;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Threading.Tasks;

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

        [HttpPut("{batchNumber}/update-ready-to-print-add-certificates", Name = "UpdateBatchLogReadyToPrintAddCertificates")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateBatchLogReadyToPrintAddCertificates(int batchNumber, [FromBody] UpdateBatchLogReadyToPrintAddCertificatesRequest request)
        {
            request.BatchNumber = batchNumber;
            return Ok(await _mediator.Send(request));
        }

        [HttpPut("{batchNumber}/update-sent-to-printer", Name = "UpdateBatchLogSentToPrinter")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ValidationResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateBatchLogSentToPrinter(int batchNumber, [FromBody] UpdateBatchLogSentToPrinterRequest request)
        {
            request.BatchNumber = batchNumber;
            return Ok(await _mediator.Send(request));
        }

        [HttpPut("{batchNumber}/update-printed", Name = "UpdateBatchPrinted")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ValidationResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateBatchLogPrinted(int batchNumber, [FromBody] UpdateBatchLogPrintedRequest request)
        {
            request.BatchNumber = batchNumber;
            return Ok(await _mediator.Send(request));
        }
    }
}