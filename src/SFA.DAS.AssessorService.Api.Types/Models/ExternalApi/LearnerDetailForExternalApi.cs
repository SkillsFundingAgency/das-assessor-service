namespace SFA.DAS.AssessorService.Api.Types.Models.ExternalApi
{
    public class LearnerDetailForExternalApi
    {
        public long Uln { get; set; }
        public int? StandardCode { get; set; }
        public string StandardReference { get; set; }
        public string PrimaryContactEmail { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public int UkPrn { get; set; }
    }
}
