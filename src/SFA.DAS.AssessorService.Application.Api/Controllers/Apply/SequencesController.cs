using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Sequences;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;

namespace SFA.DAS.AssessorService.Application.Api.Controllers.Apply
{
    public class SequencesController : Controller
    {
        private readonly IMediator _mediator;

        public SequencesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("Apply/Sequences/")]
        public async Task<ActionResult<List<SequenceSummary>>> GetAll()
        {
            return await _mediator.Send(new GetAllSequencesRequest());
        }
        
        [HttpGet("Apply/Sequences/{sequenceId}")]
        public async Task<ActionResult<Sequence>> Get(string sequenceId)
        {
            var sequence = await _mediator.Send(new GetSequenceRequest(sequenceId));
            if (sequence == null)
            {
                return NotFound();
            }

            return sequence;
        }

        [HttpPost("Apply/Sequences/")]
        [ValidateBadRequest]
        public async Task<ActionResult> Create([FromBody] Sequence sequenceSummary)
        {
            var createdSequence = await _mediator.Send(new CreateSequenceRequest(sequenceSummary));
            return Created(Url.Action("Get", new {sequenceId = createdSequence.SequenceId}),createdSequence);
        }

        [HttpDelete("Apply/Sequences/{sequenceId}")]
        public async Task<ActionResult> Delete(string sequenceId)
        {
            await _mediator.Send(new DeleteSequenceRequest(sequenceId));
            return NoContent();
        }
        
    }
}