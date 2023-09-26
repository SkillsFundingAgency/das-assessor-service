using System;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Models
{
    public class ContactModel : TestModel
    {
        public Guid? Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public Guid? OrganisationId { get; set; }
        public string Status { get; set; }
        public string Username { get; set; }
    }
}
