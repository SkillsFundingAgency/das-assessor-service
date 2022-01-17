using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class MergeOrganisation
    {
        public int Id { get; set; }

        public string PrimaryEndPointAssessorOrganisationId { get; set; }
        public string PrimaryEndpointAssessorOrganisationName { get; set; }

        public string SecondaryEndPointAssessorOrganisationId { get; set; }
        public string SecondaryEndpointAssessorOrganisationName { get; set; }
        public DateTime SecondaryEPAOEffectiveTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Status { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public Guid? ApprovedBy { get; set; }
        public Guid? CompletedBy { get; set; }
        public DateTime? CompletedAt { get; set; }

        public ICollection<MergeOrganisationStandard> MergeOrganisationStandards { get; set; }
        public ICollection<MergeOrganisationStandardVersion> MergeOrganisationStandardVersions { get; set; }
        public ICollection<MergeOrganisationStandardDeliveryArea> MergeOrganisationStandardDeliveryAreas { get; set; }
        

        public MergeOrganisation()
        {
            MergeOrganisationStandards = new List<MergeOrganisationStandard>();
            MergeOrganisationStandardVersions = new List<MergeOrganisationStandardVersion>();
            MergeOrganisationStandardDeliveryAreas = new List<MergeOrganisationStandardDeliveryArea>();
        }
    }
}
