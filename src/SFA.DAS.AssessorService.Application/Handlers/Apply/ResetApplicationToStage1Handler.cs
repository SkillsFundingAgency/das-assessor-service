using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class ResetApplicationToStage1Handler : IRequestHandler<ResetApplicationToStage1Request, bool>
    {
        private readonly IApplyRepository _applyRepository;

        public ResetApplicationToStage1Handler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }

        public async Task<bool> Handle(ResetApplicationToStage1Request request, CancellationToken cancellationToken)
        {
            return await _applyRepository.ResetApplicatonToStage1(request.Id, request.UserId);
        }
    }
}
