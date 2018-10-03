namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class EpaOrganisationStandardResponse
    {
        public EpaOrganisationStandardResponse(string details)
        {
            Details = details;
        }

        public string Details { get; set; }
    }
}