using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class AddToApplyContactAGovUkIdentifier
    {
        public string Email { get; set; }
        public string GovUkIdentifier { get; set; }
        public string ContactId { get; set; }
        public string UpdatedBy { get; set; }
    }
}
