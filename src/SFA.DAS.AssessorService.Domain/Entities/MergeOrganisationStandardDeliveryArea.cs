namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class MergeOrganisationStandardDeliveryArea
    {
        public int Id { get; set; }
        public int OrganisationStandardId { get; set; }
        public int DeliveryAreaId { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public int OrganisationStandardDeliveryAreaId { get; set; }
        public string Replicates { get; set; }

        public virtual MergeOrganisation MergeOrganisation { get; set; }
    }
}
