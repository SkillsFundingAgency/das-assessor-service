using System;

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
        public int OrganisationStandardId { get; set; }
        public string Replicates { get; set; }
      
        public virtual MergeOrganisation MergeOrganisation { get; set; }

        public int StandardCode { get; set; }


        public MergeOrganisationStandard() { }

        public MergeOrganisationStandard(OrganisationStandard sourceOrganisationStandard, string replicates)
        {
            EndPointAssessorOrganisationId = sourceOrganisationStandard.EndPointAssessorOrganisationId;
            StandardCode = sourceOrganisationStandard.StandardCode;
            StandardReference = sourceOrganisationStandard.StandardReference;
            EffectiveFrom = sourceOrganisationStandard.EffectiveFrom;
            EffectiveTo = sourceOrganisationStandard.EffectiveTo;
            DateStandardApprovedOnRegister = sourceOrganisationStandard.DateStandardApprovedOnRegister;
            Comments = sourceOrganisationStandard.Comments;
            Status = sourceOrganisationStandard.Status;
            ContactId = sourceOrganisationStandard.ContactId;
            OrganisationStandardData = sourceOrganisationStandard.OrganisationStandardData;
            OrganisationStandardId = sourceOrganisationStandard.Id;
            Replicates = replicates;
        }
    }
}
