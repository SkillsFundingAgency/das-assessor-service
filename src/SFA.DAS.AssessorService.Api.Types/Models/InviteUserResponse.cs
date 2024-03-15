using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class InviteUserResponse
    {
        public InviteUserResponse()
        {
            IsSuccess = true;
        }
        public bool IsSuccess { get; set; }
        public bool UserExists { get; set; }
        public Guid? ExistingUserId { get; set; }
    }
}
