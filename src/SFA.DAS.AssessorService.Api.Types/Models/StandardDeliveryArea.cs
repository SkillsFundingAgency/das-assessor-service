namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class EpaOrganisationStandardDeliveryArea
    {
        public int Id { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public string StandardCode { get; set; }
        public int DeliveryAreaId { get; set; }
        public string Comments { get; set; }

        public string Status { get; set; }
    }
}
