using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class StaffReport : BaseEntity
    {
        public Guid Id { get; set; }
        public string ReportName { get; set; }
        public string StoredProcedure { get; set; }
        public int? DisplayOrder { get; set; }
    }
}
