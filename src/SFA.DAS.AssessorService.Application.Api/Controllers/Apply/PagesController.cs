using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Pages;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Sections;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;

namespace SFA.DAS.AssessorService.Application.Api.Controllers.Apply
{
    public class PagesController : Controller
    {
        private readonly IMediator _mediator;

        public PagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("Apply/Sequences/{sequenceId}/Sections/{sectionId}/Pages")]
        public async Task<ActionResult<List<Page>>> GetAll(string sequenceId, string sectionId)
        {
            return await _mediator.Send(new GetPagesRequest(sequenceId, sectionId));
        }
        
        [HttpGet("Apply/Sequences/{sequenceId}/Sections/{sectionId}/Pages/{pageId}")]
        public async Task<ActionResult<Page>> Get(string sequenceId, string sectionId, string pageId)
        {
            var page = await _mediator.Send(new GetPageRequest(sequenceId, sectionId, pageId));
            if (page == null)
            {
                return NotFound();
            }

            return page;
        }

        [HttpPost("Apply/Sequences/{sequenceId}/Sections/{sectionId}/Pages")]
        [ValidateBadRequest]
        public async Task<ActionResult> Create(string sequenceId, string sectionId, [FromBody] Page page)
        {
            var createdPage = await _mediator.Send(new CreatePageRequest(sequenceId, sectionId, page));
            return Created(Url.Action("Get", new {sequenceId, sectionId, createdPage.PageId}), createdPage);
        }

        [HttpDelete("Apply/Sequences/{sequenceId}/Sections/{sectionId}/Pages/{pageId}")]
        public async Task<ActionResult> Delete(string sequenceId, string sectionId, string pageId)
        {
            await _mediator.Send(new DeletePageRequest(sequenceId, sectionId, pageId));
            return NoContent();
        }
        
    }
    
}