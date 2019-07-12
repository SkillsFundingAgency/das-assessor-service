using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.AssessorService.Web.Controllers.ManageUsers.ViewModels
{
  public class InviteContactViewModel
  {
    [Required(ErrorMessage = "Enter a given name")]
    public string GivenName { get; set; }
    [Required(ErrorMessage = "Enter a family name")]
    public string FamilyName { get; set; }
    [Required(ErrorMessage = "Enter an email address")]
    [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
    public string Email { get; set; }
    public EditPrivilegesViewModel PrivilegesViewModel { get; set; }
  }
}