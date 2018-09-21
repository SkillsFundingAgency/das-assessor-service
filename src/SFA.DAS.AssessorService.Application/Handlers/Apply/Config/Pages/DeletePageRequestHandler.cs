using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Pages;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Sections;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Config.Pages
{
    public class DeletePageRequestHandler : IRequestHandler<DeletePageRequest>
    {
        private readonly IApplyRepository _applyRepository;

        public DeletePageRequestHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }
        
        public async Task Handle(DeletePageRequest message, CancellationToken cancellationToken)
        {
            var sequences = await _applyRepository.GetWorkflowDefinition();
            var thisSequence = sequences.FirstOrDefault(s => s.SequenceId == message.SequenceId);
           
            if (thisSequence == null)
            {
                throw new BadRequestException("Sequence does not exist");
            }
            
            var section = thisSequence.Sections.FirstOrDefault(s => s.SectionId == message.SectionId);
            
            if (section == null)
            {
                throw new BadRequestException($"Section {message.SectionId} does not exist in Sequence {message.SequenceId}");
            }
            
            var pages = section.Pages;
            
            pages.RemoveAll(s => s.PageId == message.PageId);

            var newOrder = 0;
            pages.ForEach(s => s.Order = newOrder++);

            var workflowJson = JsonConvert.SerializeObject(sequences);

            await _applyRepository.UpdateWorkflowDefinition(workflowJson);
        }
    }
}