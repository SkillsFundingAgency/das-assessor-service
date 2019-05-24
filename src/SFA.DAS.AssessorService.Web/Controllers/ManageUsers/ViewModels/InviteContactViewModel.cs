using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.AssessorService.Web.Controllers.ManageUsers.ViewModels
{
    public class InviteContactViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        public string GivenName { get; set; }
        [Required(ErrorMessage = "Family name is required")]
        public string FamilyName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email must be valid")]
        public string Email { get; set; }
        public EditPrivilegesViewModel PrivilegesViewModel { get; set; }
    }
}