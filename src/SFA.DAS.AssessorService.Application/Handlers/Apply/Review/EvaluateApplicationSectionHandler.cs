﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Review
{
    public class EvaluateApplicationSectionHandler : IRequestHandler<EvaluateApplicationSectionRequest, Unit>
    {
        private readonly IApplyRepository _applyRepository;

        public EvaluateApplicationSectionHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }

        public async Task<Unit> Handle(EvaluateApplicationSectionRequest request, CancellationToken cancellationToken)
        {
            await _applyRepository.EvaluateApplicationSection(request.ApplicationId, request.SequenceNo, request.SectionNo, request.IsSectionComplete, request.EvaluatedBy);

            return Unit.Value;
        }
    }
}
