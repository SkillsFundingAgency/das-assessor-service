using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.AssessorService.Web.ViewModels
{
    public class ChangeAddressViewModel : ChangeOrganisationDetailsViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter a building and street")]
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter a town or city")]
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter a postcode")]
        public string Postcode { get; set; }
    }
}
