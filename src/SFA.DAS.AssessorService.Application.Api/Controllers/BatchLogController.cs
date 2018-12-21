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
using Swashbuckle.AspNetCore.SwaggerGen;

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

        [HttpPost(Name = "CreateBatchLog")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(EpaOrganisationResponse))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.Gone, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CreateBatchLog([FromBody] CreateBatchLogRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpPut("update-batch-data",Name="UpdateBatchDataBatchLog")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ValidationResponse))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.Gone, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateBatchLogWithBatchData([FromBody] UpdateBatchLogBatchDataRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

    }
} 