namespace SFA.DAS.AssessorService.Api.Types.Models.UserManagement
{
    public class RemoveContactFromOrganisationResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public bool SelfRemoved { get; set; }
    }
}