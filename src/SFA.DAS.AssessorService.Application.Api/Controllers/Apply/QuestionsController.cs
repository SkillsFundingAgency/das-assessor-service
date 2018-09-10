using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Handlers.Apply.SequenceSummary;

namespace SFA.DAS.AssessorService.Application.Api.Controllers.Apply
{
    [Route("api/v1/questions/")]
    public class QuestionsController : Controller
    {
        private readonly IMediator _mediator;

        public QuestionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("summary/{userId}")]
        public async Task<ActionResult<List<SequenceSummary>>> Summary(int userId)
        {
            var sequenceSummaries = await _mediator.Send(new SequenceSummaryRequest(userId));
            return sequenceSummaries;
        }
    }
}