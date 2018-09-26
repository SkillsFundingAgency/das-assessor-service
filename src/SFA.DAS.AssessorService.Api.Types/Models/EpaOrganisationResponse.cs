namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class EpaOrganisationResponse
    {
        public EpaOrganisationResponse(string details)
        {
            Details = details;
        }

        public string Details { get; set; }
    }
}
