namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class EpaOrganisationContactResponse
    {
        public EpaOrganisationContactResponse(string details)
        {
            Details = details;
        }

        public string Details { get; set; }
    }
}