using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class OrganisationStandard
    {
        public int Id { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public int StandardCode { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? DateStandardApprovedOnRegister { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public Guid? ContactId { get; set; }
        public string StandardReference { get; set; }
        public virtual Organisation Organisation { get; set; }
        public virtual ICollection<OrganisationStandardVersion> OrganisationStandardVersions { get; set; }
        public virtual ICollection<OrganisationStandardDeliveryArea> OrganisationStandardDeliveryAreas { get; set; }
    }
}