using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Financial.Review
{
    public class StartFinancialReviewRequest : IRequest
    {
        public Guid ApplicationId { get; }

        public StartFinancialReviewRequest(Guid applicationId)
        {
            ApplicationId = applicationId;
        }
    }
}
