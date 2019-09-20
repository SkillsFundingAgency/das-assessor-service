using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.AssessorService.Web.ViewModels
{
    public class ChangePhoneNumberViewModel : ChangeOrganisationDetailsViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter a phone number")]
        public string PhoneNumber { get; set; }
    }
}
