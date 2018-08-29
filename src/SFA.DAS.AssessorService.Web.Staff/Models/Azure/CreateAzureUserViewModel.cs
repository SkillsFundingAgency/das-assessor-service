using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.AssessorService.Web.Staff.Models.Azure
{
    public class CreateAzureUserViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter valid First Name")]
        public string FirstName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter valid Last Name")]
        public string LastName { get; set; }
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid Email")]
        public string Email { get; set; }
        [RegularExpression(@"^([1-9][\d]{7})$", ErrorMessage = "Please enter valid UKPRN")]
        public string UkPrn { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select a Product")]
        public string ProductId { get; set; }
        public List<Product> Products { set; get; }
    }
}
