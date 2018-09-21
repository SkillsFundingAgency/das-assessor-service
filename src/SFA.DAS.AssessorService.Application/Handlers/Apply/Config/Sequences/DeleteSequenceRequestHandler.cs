using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Sequences;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Config.Sequences
{
    public class DeleteSequenceRequestHandler : IRequestHandler<DeleteSequenceRequest>
    {
        private readonly IApplyRepository _applyRepository;

        public DeleteSequenceRequestHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }
        
        public async Task Handle(DeleteSequenceRequest message, CancellationToken cancellationToken)
        {
            var sequences = await _applyRepository.GetWorkflowDefinition();

            sequences.RemoveAll(s => s.SequenceId == message.SequenceId);

            var newOrder = 0;
            sequences.ForEach(s => s.Order = newOrder++);

            var workflowJson = JsonConvert.SerializeObject(sequences);

            await _applyRepository.UpdateWorkflowDefinition(workflowJson);
        }
    }
}