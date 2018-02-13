using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class Organisation
    {
        [Key]
        public long OrganisationId { get; set; }
        public string EpaoOrgId { get; set; }
        public string UkPrn { get; set; }
        public string Name { get; set; }

        [ForeignKey("PrimaryContactId")]
        public Contact PrimaryContact{ get; set; }
        public long PrimaryContactId { get; set; }

        public OrganisationStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}