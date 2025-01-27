using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Financial.Review
{
    public class ReturnFinancialReviewRequest : IRequest<Unit>
    {
        public Guid Id { get; }
        public Domain.Entities.FinancialGrade UpdatedGrade { get; }

        public ReturnFinancialReviewRequest(Guid id, Domain.Entities.FinancialGrade updatedGrade)
        {
            Id = id;
            UpdatedGrade = updatedGrade;
        }
    }
}
