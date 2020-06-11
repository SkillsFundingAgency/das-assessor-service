namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class DeliveryArea
    {
        public int Id { get; set; }
        public string Area { get; set; }
        public string Status { get; set; }
        public int Ordering { get; set; }
        public virtual OrganisationStandardDeliveryArea OrganisationStandardDeliveryArea { get; set; }
    }
}