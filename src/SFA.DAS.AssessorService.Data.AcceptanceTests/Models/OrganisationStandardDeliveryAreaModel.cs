namespace SFA.DAS.AssessorService.Data.IntegrationTests.Models
{
    public class OrganisationStandardDeliveryAreaModel: TestModel
    {
        public int Id { get; set; }
        public int OrganisationStandardId { get; set; }
        public int DeliveryAreaId { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
    }
}