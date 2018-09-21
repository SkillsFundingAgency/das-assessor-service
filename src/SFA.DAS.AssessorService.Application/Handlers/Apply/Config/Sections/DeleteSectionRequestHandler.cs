using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Sections;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Config.Sections
{
    public class DeleteSectionRequestHandler : IRequestHandler<DeleteSectionRequest>
    {
        private readonly IApplyRepository _applyRepository;

        public DeleteSectionRequestHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }
        
        public async Task Handle(DeleteSectionRequest message, CancellationToken cancellationToken)
        {
            var sequences = await _applyRepository.GetWorkflowDefinition();
            var thisSequence = sequences.FirstOrDefault(s => s.SequenceId == message.SequenceId);
           
            if (thisSequence == null)
            {
                throw new BadRequestException("Sequence does not exist");
            }
            
            var sections = thisSequence.Sections;
            
            sections.RemoveAll(s => s.SectionId == message.SectionId);

            var newOrder = 0;
            sections.ForEach(s => s.Order = newOrder++);

            var workflowJson = JsonConvert.SerializeObject(sequences);

            await _applyRepository.UpdateWorkflowDefinition(workflowJson);
        }
    }
}