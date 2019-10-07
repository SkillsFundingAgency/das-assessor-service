using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading.Tasks;
using System.Threading;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.ApplyTypes;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Review
{
    public class StartApplicationSequenceReviewHandler : IRequestHandler<StartApplicationSequenceReviewRequest>
    {
        private readonly IApplyRepository _applyRepository;

        public StartApplicationSequenceReviewHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }

        public async Task<Unit> Handle(StartApplicationSequenceReviewRequest request, CancellationToken cancellationToken)
        {
            var application = await _applyRepository.GetApplication(request.ApplicationId);

            if (application != null && application.ReviewStatus == ApplicationReviewStatus.New)
            {
                var sequence = application.ApplyData.Sequences.FirstOrDefault(s => s.SequenceNo == request.SequenceNo);

                if (sequence != null)
                {
                    await _applyRepository.StartApplicationSequenceReview(application.Id, sequence.SequenceNo);

                    if (sequence.SequenceNo == 1)
                    {
                        await _applyRepository.UpdateApplicationSectionStatus(application.Id, "0", "0", ApplicationSectionStatus.InProgress);
                        await _applyRepository.UpdateApplicationSectionStatus(application.Id, "0", "1", ApplicationSectionStatus.InProgress);
                    }
                    else if(sequence.SequenceNo == 2)
                    {
                        await _applyRepository.UpdateApplicationSectionStatus(application.Id, "1", "0", ApplicationSectionStatus.InProgress);
                    }
                }
            }

            return Unit.Value;
        }
    }
}
