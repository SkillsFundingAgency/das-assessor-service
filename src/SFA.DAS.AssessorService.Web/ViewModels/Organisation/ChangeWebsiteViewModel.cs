using System;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.AssessorService.Web.ViewModels
{
    public class ChangeWebsiteViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter a website address")]
        public string WebsiteLink { get; set; }
        public string ActionChoice { get; set; }
    }
}
