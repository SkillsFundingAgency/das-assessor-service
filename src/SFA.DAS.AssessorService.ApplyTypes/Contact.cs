using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class Contact
    {
        public string Email { get; set; }
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public Guid? SigninId { get; set; }
        public string SigninType { get; set; }
        public Guid? ApplyOrganisationId { get; set; }
        public bool IsApproved { get; set; }
        public Guid Id { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime DeletedAt { get; set; }
        public string DeletedBy { get; set; }
    }
}
