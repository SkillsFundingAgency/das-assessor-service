using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class SigninIdWrapper
    {
        public SigninIdWrapper(Guid signinId)
        {
            SigninId = signinId;
        }

        public Guid SigninId { get; set; }
        
    }
}
