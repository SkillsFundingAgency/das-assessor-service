namespace SFA.DAS.AssessorService.ExternalApiDataSync.Data.Entities
{
    public class OrganisationStandardDeliveryArea
    {
        public int Id { get; set; }
        public int OrganisationStandardId { get; set; }
        public int DeliveryAreaId { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
    }
}
