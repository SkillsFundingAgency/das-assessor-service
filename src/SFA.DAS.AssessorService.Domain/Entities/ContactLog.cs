using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class ContactLog 
    {
        public DateTime DateTime { get; set; }
        public Guid User { get; set; }
        public Guid ContactId { get; set; }
        public string ContactLogType { get; set; }
        public string ContactLogDetails { get; set; }
    }
    
    public class ContactLogType
    {
        public const string PrivilegesAmended = "PrivilegesAmended";
    }
}