using SFA.DAS.GovUK.Auth.Models;

namespace SFA.DAS.AssessorService.Web.Models;

public class StubAuthenticationViewModel : StubAuthUserDetails
{
    public string ReturnUrl { get; set; }   
}