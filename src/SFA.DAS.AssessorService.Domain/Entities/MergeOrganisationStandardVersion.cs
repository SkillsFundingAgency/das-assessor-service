using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class MergeOrganisationStandardVersion
    {
        public int Id { get; set; }

        public string StandardUid { get; set; }
        public string Version { get; set; }
        public int OrganisationStandardId { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? DateVersionApproved { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }

        public string Replicates { get; set; }

        public virtual MergeOrganisation MergeOrganisation { get; set; }
    }
}
