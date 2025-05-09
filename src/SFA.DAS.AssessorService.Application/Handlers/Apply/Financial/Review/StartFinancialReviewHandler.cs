﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Financial.Review;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Financial.Review
{
    public class StartFinancialReviewHandler : IRequestHandler<StartFinancialReviewRequest, Unit>
    {
        private readonly IApplyRepository _applyRepository;

        public StartFinancialReviewHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }

        public async Task<Unit> Handle(StartFinancialReviewRequest request, CancellationToken cancellationToken)
        {
            await _applyRepository.StartFinancialReview(request.Id, request.Reviewer);

            return Unit.Value;
        }
    }
}
