using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Pages;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Questions;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Sections;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;

namespace SFA.DAS.AssessorService.Application.Api.Controllers.Apply
{
    public class QuestionsController : Controller
    {
        private readonly IMediator _mediator;

        public QuestionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("Apply/Sequences/{sequenceId}/Sections/{sectionId}/Pages/{PageId}/Questions")]
        public async Task<ActionResult<List<Question>>> GetAll(string sequenceId, string sectionId, string pageId)
        {
            return await _mediator.Send(new GetQuestionsRequest(sequenceId, sectionId, pageId));
        }
        
        [HttpGet("Apply/Sequences/{sequenceId}/Sections/{sectionId}/Pages/{pageId}/Questions/{QuestionId}")]
        public async Task<ActionResult<Question>> Get(string sequenceId, string sectionId, string pageId, string questionId)
        {
            var question = await _mediator.Send(new GetQuestionRequest(sequenceId, sectionId, pageId, questionId));
            if (question == null)
            {
                return NotFound();
            }

            return question;
        }

        [HttpPost("Apply/Sequences/{sequenceId}/Sections/{sectionId}/Pages/{PageId}/Questions")]
        [ValidateBadRequest]
        public async Task<ActionResult> Create(string sequenceId, string sectionId, string pageId, [FromBody] Question question)
        {
            var createdQuestion = await _mediator.Send(new CreateQuestionRequest(sequenceId, sectionId, pageId, question));
            return Created(Url.Action("Get", new {sequenceId, sectionId, pageId, createdQuestion.QuestionId}), createdQuestion);
        }

        [HttpDelete("Apply/Sequences/{sequenceId}/Sections/{sectionId}/Pages/{pageId}/Questions/{QuestionId}")]
        public async Task<ActionResult> Delete(string sequenceId, string sectionId, string pageId, string questionId)
        {
            await _mediator.Send(new DeleteQuestionRequest(sequenceId, sectionId, pageId, questionId));
            return NoContent();
        }
        
    }
    
}