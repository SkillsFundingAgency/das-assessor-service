namespace SFA.DAS.AssessorService.Api.Types.Models.Azure
{
    public class CreateAzureUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public string Password { get; set; }
    }
}
