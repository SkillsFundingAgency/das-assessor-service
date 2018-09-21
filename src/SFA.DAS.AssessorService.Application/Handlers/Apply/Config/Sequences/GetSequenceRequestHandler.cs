using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Sequences;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Config.Sequences
{
    public class GetSequenceRequestHandler : IRequestHandler<GetSequenceRequest, Sequence>
    {
        private readonly IApplyRepository _applyRepository;

        public GetSequenceRequestHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }
        
        public async Task<Sequence> Handle(GetSequenceRequest request, CancellationToken cancellationToken)
        {
            var sequences = await _applyRepository.GetWorkflowDefinition();
            return sequences.FirstOrDefault(s => s.SequenceId == request.SequenceId);
        }
    }
}