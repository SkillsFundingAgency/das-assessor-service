namespace SFA.DAS.AssessorService.Web.ViewModels.Account
{
    public class CreateAccountViewModel : AccountViewModel
    {
        
        public string Email { get; set; }

        public override string ToString()
        {
            return $"{GivenName} {FamilyName}, {Email}";
        }
    }
}
