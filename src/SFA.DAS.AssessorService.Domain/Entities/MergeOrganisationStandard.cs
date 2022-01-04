using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class MergeOrganisationStandard
    {
        public int Id { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? DateStandardApprovedOnRegister { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public Guid? ContactId { get; set; }
        public string OrganisationStandardData { get; set; }
        public string StandardReference { get; set; }
        public string Replicates { get; set; }

        

        


        public virtual MergeOrganisation MergeOrganisation { get; set; }



        public string ReferenceNumber { get; set; }

    }
}
