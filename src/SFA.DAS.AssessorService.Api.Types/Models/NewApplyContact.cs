using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class NewApplyContact
    {
        public string Email { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public bool FromAssessor { get; set; }
    }
}
