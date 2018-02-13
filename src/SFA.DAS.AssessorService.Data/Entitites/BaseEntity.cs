namespace SFA.DAS.AssessorService.Data.Entitites
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
