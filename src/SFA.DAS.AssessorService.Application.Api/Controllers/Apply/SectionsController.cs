using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Sections;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;

namespace SFA.DAS.AssessorService.Application.Api.Controllers.Apply
{
    public class SectionsController : Controller
    {
        private readonly IMediator _mediator;

        public SectionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("Apply/Sequences/{sequenceId}/Sections")]
        public async Task<ActionResult<List<SectionSummary>>> GetAll(string sequenceId)
        {
            return await _mediator.Send(new GetSectionsRequest(sequenceId));
        }
        
        [HttpGet("Apply/Sequences/{sequenceId}/Sections/{sectionId}")]
        public async Task<ActionResult<Section>> Get(string sequenceId, string sectionId)
        {
            var section = await _mediator.Send(new GetSectionRequest(sequenceId, sectionId));
            if (section == null)
            {
                return NotFound();
            }

            return section;
        }

        [HttpPost("Apply/Sequences/{sequenceId}/Sections")]
        [ValidateBadRequest]
        public async Task<ActionResult> Create(string sequenceId, [FromBody] Section section)
        {
            var createdSection = await _mediator.Send(new CreateSectionRequest(sequenceId, section));
            return Created(Url.Action("Get", new {sequenceId = sequenceId, sectionId = createdSection.SectionId}),createdSection);
        }

        [HttpDelete("Apply/Sequences/{sequenceId}/Sections/{sectionId}")]
        public async Task<ActionResult> Delete(string sequenceId, string sectionId)
        {
            await _mediator.Send(new DeleteSectionRequest(sequenceId, sectionId));
            return NoContent();
        }
        
    }
}