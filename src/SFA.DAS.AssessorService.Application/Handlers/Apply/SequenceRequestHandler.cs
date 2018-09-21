using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class SequenceRequestHandler : IRequestHandler<SequenceRequest, Sequence>
    {
        private readonly IApplyRepository _applyRepository;

        public SequenceRequestHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }
        
        public async Task<Sequence> Handle(SequenceRequest request, CancellationToken cancellationToken)
        {
            var workflow = await _applyRepository.GetSequences(request.UserId);
            
            var sequence = workflow.Single(w => w.SequenceId == request.SequenceId);

            if (!sequence.Active)
            {
                throw new BadRequestException("This sequence is not active");
            }
            
            return sequence;
        }
    }
}