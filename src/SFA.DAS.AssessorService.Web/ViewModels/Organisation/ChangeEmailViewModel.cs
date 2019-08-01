using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.AssessorService.Web.ViewModels
{
    public class ChangeEmailViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter an email address")]
        public string Email { get; set; }
        public string ActionChoice { get; set; }
    }
}
