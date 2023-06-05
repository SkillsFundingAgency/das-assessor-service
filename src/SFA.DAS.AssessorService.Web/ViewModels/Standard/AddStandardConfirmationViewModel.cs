using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.ViewModels.Standard
{
    public class AddStandardConfirmationViewModel
    {
        public StandardVersion Standard {get; set;}
        
        public List<string> StandardVersions { get; set; }
        
        public string FeedbackUrl { get; set; }

        public string StandardTitle => Standard?.Title;
        
        public string StandardReference => Standard?.IFateReferenceNumber;
        
        public string ConfirmedVersionsText => $"Version{(StandardVersions.Count > 1 ? "s" : string.Empty)} {string.Join(", ", StandardVersions)}";
    }
}
