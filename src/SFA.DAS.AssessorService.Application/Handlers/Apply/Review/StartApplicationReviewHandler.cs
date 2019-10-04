using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading.Tasks;
using System.Threading;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Review
{
    public class StartApplicationReviewHandler : IRequestHandler<StartApplicationReviewRequest>
    {
        private readonly IApplyRepository _applyRepository;

        public StartApplicationReviewHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }

        public async Task<Unit> Handle(StartApplicationReviewRequest request, CancellationToken cancellationToken)
        {
            await _applyRepository.StartApplicationReview(request.ApplicationId, request.SequenceNo);

            return Unit.Value;
        }
    }
}
