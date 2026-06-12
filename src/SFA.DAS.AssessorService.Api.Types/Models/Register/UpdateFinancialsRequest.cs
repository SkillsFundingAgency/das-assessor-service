using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Register
{
    public class UpdateFinancialsRequest : IRequest<Unit>
    {
        public string EpaOrgId { get; set; }
        public DateTime? FinancialDueDate { get; set; }
        public bool? FinancialExempt { get; set; }
    }
}