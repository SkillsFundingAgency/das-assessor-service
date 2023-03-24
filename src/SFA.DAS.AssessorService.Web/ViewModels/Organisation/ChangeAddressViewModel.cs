using SFA.DAS.AssessorService.Web.Validators;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.AssessorService.Web.ViewModels
{
    public class ChangeAddressViewModel : ChangeOrganisationDetailsViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter a building and street")]
        [NoHtmlTags(ErrorMessage = "The first line of address contains invalid characters")]
        public string AddressLine1 { get; set; }

        [NoHtmlTags(ErrorMessage = "The second line of address contains invalid characters")]
        public string AddressLine2 { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter a town or city")]
        [NoHtmlTags(ErrorMessage = "The town or city contains invalid characters")]
        public string AddressLine3 { get; set; }

        [NoHtmlTags(ErrorMessage = "The county contains invalid characters")]
        public string AddressLine4 { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter a postcode")]
        [NoHtmlTags(ErrorMessage = "The postcode contains invalid characters")]
        public string Postcode { get; set; }
    }
}
