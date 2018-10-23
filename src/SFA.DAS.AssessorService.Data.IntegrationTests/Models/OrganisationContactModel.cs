using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Models
{
    public class OrganisationContactModel : TestModel
    {
        public Guid Id { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public Guid OrganisationId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string PhoneNumber { get; set; }
    }
}
