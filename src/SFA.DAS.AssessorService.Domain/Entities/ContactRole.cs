using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class ContactRole
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; }
        public Guid ContactId { get; set; }
    }
}