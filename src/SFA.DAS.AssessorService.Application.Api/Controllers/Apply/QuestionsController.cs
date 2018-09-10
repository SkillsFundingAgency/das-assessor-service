using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Handlers.Apply.Page;
using SFA.DAS.AssessorService.Application.Handlers.Apply.Sequence;
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
        
        [HttpGet("sequence/{userId}/{sequenceId}")]
        public async Task<ActionResult<Sequence>> Section(int userId, string sequenceId)
        {
            var sequence = await _mediator.Send(new SequenceRequest(userId, sequenceId));
            return sequence;
        }
        
        [HttpGet("page/{userId}/{pageId}")]
        public async Task<ActionResult<Page>> Page(int userId, string pageId)
        {
            var page = await _mediator.Send(new PageRequest(userId, pageId));
            return page;
        }
    }
}