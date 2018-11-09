namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class OrganisationStandardDeliveryArea
    {
        public int Id { get; set; }
        public int OrganisationStandardId { get; set; }
        public int DeliveryAreaId { get; set; }
        public string DeliveryArea { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
    }
}
