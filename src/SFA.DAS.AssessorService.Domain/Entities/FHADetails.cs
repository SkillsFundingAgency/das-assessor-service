using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class FHADetails
    {
        public DateTime? FinancialDueDate { get; set; }
        public bool? FinancialExempt { get; set; }
    }
}
