using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Web.ViewModels
{
    public class ChangePhoneNumberViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter a phone number")]
        public string PhoneNumber { get; set; }
        public List<ContactResponse> Contacts { get; internal set; }
        public string ActionChoice { get; set; }
    }
}
