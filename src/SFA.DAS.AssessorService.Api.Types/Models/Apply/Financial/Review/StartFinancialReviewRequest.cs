using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Financial.Review
{
    public class StartFinancialReviewRequest : IRequest
    {
        public Guid Id { get; }
        public string Reviewer { get; }

        public StartFinancialReviewRequest(Guid id, string reviewer)
        {
            Id = id;
            Reviewer = reviewer;
        }
    }
}
