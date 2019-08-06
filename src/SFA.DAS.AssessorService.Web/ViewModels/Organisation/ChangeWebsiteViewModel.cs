using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Web.ViewModels
{
    public class ChangeWebsiteViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter a website address")]
        public string WebsiteLink { get; set; }
        public string ActionChoice { get; set; }
        public List<ContactsWithPrivilegesResponse> Contacts { get; internal set; }
    }
}
