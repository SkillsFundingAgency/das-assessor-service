using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.AssessorService.Web.ViewModels
{
    public class ChangeWebsiteViewModel : ChangeOrganisationDetailsViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter a website address")]
        public string WebsiteLink { get; set; }
    }
}
