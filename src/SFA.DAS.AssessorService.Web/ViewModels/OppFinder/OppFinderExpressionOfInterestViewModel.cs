using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Web.ViewModels.OppFinder
{
    public class OppFinderExpressionOfInterestViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter an email address")]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter an organisation name ")]
        public string OrganisationName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter a name")]
        public string ContactName { get; set; }
        public string ContactNumber { get; set; }

        public int? StandardCode { get; set; }
        public string StandardName { get; set; }
        public string StandardLevel { get; set; }
        public string StandardReference { get; set; }
        public string StandardSector { get; set; }

        public StandardStatus StandardStatus { get; set; }
    }
}
