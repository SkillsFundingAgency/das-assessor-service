using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Review
{
    public class StartApplicationSectionReviewHandler :IRequestHandler<StartApplicationSectionReviewRequest, Unit>
    {
        private readonly IApplyRepository _applyRepository;

        public StartApplicationSectionReviewHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }

        public async Task<Unit> Handle(StartApplicationSectionReviewRequest request, CancellationToken cancellationToken)
        {
            await _applyRepository.StartApplicationSectionReview(request.ApplicationId, request.SequenceNo, request.SectionNo, request.Reviewer);

            return Unit.Value;
        }
    }
}
