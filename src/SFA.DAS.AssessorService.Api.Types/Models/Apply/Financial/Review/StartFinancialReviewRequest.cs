using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Financial.Review
{
    public class StartFinancialReviewRequest : IRequest
    {
        public Guid Id { get; }

        public StartFinancialReviewRequest(Guid id)
        {
            Id = id;
        }
    }
}
