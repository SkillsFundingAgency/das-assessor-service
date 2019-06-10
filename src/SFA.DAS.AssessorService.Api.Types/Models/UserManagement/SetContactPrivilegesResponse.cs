namespace SFA.DAS.AssessorService.Api.Types.Models.UserManagement
{
    public class SetContactPrivilegesResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public bool HasRemovedOwnUserManagement { get; set; }
    }
}