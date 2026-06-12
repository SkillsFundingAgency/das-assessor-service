using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class OfsOrganisation
    {
        public Guid Id { get; set; }
        public int Ukprn { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
