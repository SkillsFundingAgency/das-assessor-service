using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.Apply
{
    public class StandardVersionViewModel
    {
        public string StandardToFind { get; set; }

        public string StandardReference { get; set; }

        public List<StandardVersion> Results { get; set; }

        public StandardVersion SelectedStandard { get; set; }

        public List<string> SelectedVersions { get; set; }

        public bool IsConfirmed { get; set; }

        public string ApplicationStatus { get; set; }
    }


}
