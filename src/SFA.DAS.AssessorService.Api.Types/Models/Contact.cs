namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using Domain.Enums;

    public class Contact
    {
        public string EndPointAssessorOrganisationId { get; set; }

        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }

        public ContactStatus ContactStatus { get; set; }
    }
}
