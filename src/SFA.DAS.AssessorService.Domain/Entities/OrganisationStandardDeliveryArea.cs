namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class OrganisationStandardDeliveryArea
    {
        public int Id { get; set; }
        public int OrganisationStandardId { get; set; }
        public int DeliveryAreaId { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public virtual DeliveryArea DeliveryArea { get; set; }
        public virtual OrganisationStandard OrganisationStandard { get; set; }
    }
}