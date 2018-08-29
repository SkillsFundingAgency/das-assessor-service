using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure.Azure.Requests
{
    public class AzureCreateUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        [JsonIgnore] public string Password { get; set; }
    }
}
