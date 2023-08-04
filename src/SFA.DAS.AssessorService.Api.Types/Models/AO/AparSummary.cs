namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class AparSummary
    {
        public string Id { get; set; } 
        public string Name { get; set; }
        public long? Ukprn { get; set; }
        public OrganisationData OrganisationData { get; set; }

        public int? OrganisationTypeId { get; set; }
        public string OrganisationType { get; set; }
        public string Status { get; set; }
        public string Email { get; set; }
    }
}
