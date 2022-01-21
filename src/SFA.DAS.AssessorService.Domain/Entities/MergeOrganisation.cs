using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class MergeOrganisation
    {
        public int Id { get; set; }

        public string PrimaryEndPointAssessorOrganisationId { get; set; }
        public string PrimaryEndPointAssessorOrganisationName { get; set; }

        public string SecondaryEndPointAssessorOrganisationId { get; set; }
        public string SecondaryEndPointAssessorOrganisationName { get; set; }
        public DateTime SecondaryEPAOEffectiveTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string ApprovedBy { get; set; }
        public string CompletedBy { get; set; }
        public DateTime? CompletedAt { get; set; }

        public ICollection<MergeOrganisationStandard> MergeOrganisationStandards { get; set; }
        public ICollection<MergeOrganisationStandardVersion> MergeOrganisationStandardVersions { get; set; }
        public ICollection<MergeOrganisationStandardDeliveryArea> MergeOrganisationStandardDeliveryAreas { get; set; }
        public ICollection<MergeApply> MergeSecondaryApplications { get; set; }


        [NotMapped]
        public string PrimaryOrganisationEmail { get; set; }
        [NotMapped]
        public string SecondaryOrganisationEmail { get; set; }
        [NotMapped]
        public string PrimaryContactName { get; set; }
        [NotMapped]
        public string SecondaryContactName { get; set; }

        public MergeOrganisation()
        {
            MergeOrganisationStandards = new List<MergeOrganisationStandard>();
            MergeOrganisationStandardVersions = new List<MergeOrganisationStandardVersion>();
            MergeOrganisationStandardDeliveryAreas = new List<MergeOrganisationStandardDeliveryArea>();
            MergeSecondaryApplications = new List<MergeApply>();
        }
    }
}
