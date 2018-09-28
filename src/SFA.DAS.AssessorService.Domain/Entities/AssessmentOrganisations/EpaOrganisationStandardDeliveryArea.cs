namespace SFA.DAS.AssessorService.Domain.Entities.AssessmentOrganisations
{
    public class EpaOrganisationStandardDeliveryArea
    {
        public int Id { get; set; }
        public int OrganisationStandardId { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public int StandardCode { get; set; }
        public int DeliveryAreaId { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
    }
}
