using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Financial.Review
{
    public class StartFinancialReviewRequest : IRequest
    {
        public Guid Id { get; }
        public string StartedBy { get; set; }

        public StartFinancialReviewRequest(Guid id, string startedBy)
        {
            Id = id;
            StartedBy = startedBy;
        }
    }
}
