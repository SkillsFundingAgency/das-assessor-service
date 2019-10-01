using MediatR;
using SFA.DAS.AssessorService.ApplyTypes;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Financial.Review
{
    public class UpdateGradeRequest : IRequest
    {
        public Guid Id { get; }
        public Guid OrgId { get; }
        public FinancialGrade UpdatedGrade { get; }

        public UpdateGradeRequest(Guid id, Guid orgId, FinancialGrade updatedGrade)
        {
            Id = id;
            OrgId = orgId;
            UpdatedGrade = updatedGrade;
        }
    }
}
