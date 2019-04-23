using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class AddToApplyContactASignInId
    {
        public string Email { get; set; }
        public string SignInId { get; set; }
        public string ContactId { get; set; }
        public string UpdatedBy { get; set; }
    }
}
