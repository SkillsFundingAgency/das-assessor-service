using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Financial.Review;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Financial.Review
{
    public class StartFinancialReviewHandler : IRequestHandler<StartFinancialReviewRequest>
    {
        private readonly IApplyRepository _applyRepository;

        public StartFinancialReviewHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }

        public async Task<Unit> Handle(StartFinancialReviewRequest request, CancellationToken cancellationToken)
        {
            var application = await _applyRepository.GetApplication(request.Id);

            if (application != null && application.FinancialReviewStatus == FinancialReviewStatus.New)
            {
                int financialSequenceNo = 1;
                var sequence = application.ApplyData.Sequences.FirstOrDefault(s => s.SequenceNo == financialSequenceNo);

                if (sequence != null)
                {
                    await _applyRepository.StartFinancialReview(application.Id);
                }
            }

            return Unit.Value;
        }
    }
}
