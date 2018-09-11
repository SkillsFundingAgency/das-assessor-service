using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;

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
        public async Task<ActionResult<List<SequenceSummary>>> Summary(string userId)
        {
            var sequenceSummaries = await _mediator.Send(new SequenceSummaryRequest(userId));
            return sequenceSummaries;
        }
        
        [HttpGet("sequence/{userId}/{sequenceId}")]
        public async Task<ActionResult<Sequence>> Section(string userId, string sequenceId)
        {
            var sequence = await _mediator.Send(new SequenceRequest(userId, sequenceId));
            return sequence;
        }
        
        [HttpGet("page/{userId}/{pageId}")]
        public async Task<ActionResult<Page>> Page(string userId, string pageId)
        {
            var page = await _mediator.Send(new PageRequest(userId, pageId));
            return page;
        }

        [HttpPost("page/{userId}/{pageId}")]
        public async Task<ActionResult<Page>> Page(string userId, string pageId, [FromBody] List<Question> updatedQuestions)
        {
            var updatedPage = await _mediator.Send(new UpdatePageRequest(userId, pageId, updatedQuestions));
            return updatedPage;
        }
    }

    public class QuestionOutputUpdateRequest
    {
        public List<Question> Questions { get; set; }
    }
}