using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class ContactRole
    {
        public Guid Guid { get; set; }
        public Guid ContactId { get; set; }
        public string Role { get; set; }
    }
}