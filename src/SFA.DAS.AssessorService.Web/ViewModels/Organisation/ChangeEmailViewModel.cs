using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Web.ViewModels
{
    public class ChangeEmailViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter an email address")]
        public string Email { get; set; }
        public List<ContactsWithPrivilegesResponse> Contacts { get; internal set; }
        public string ActionChoice { get; set; }
    }
}
