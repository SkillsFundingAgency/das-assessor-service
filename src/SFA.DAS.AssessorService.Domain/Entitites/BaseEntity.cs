namespace SFA.DAS.AssessorService.Domain.Entities
{
    using System;

    public class BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
