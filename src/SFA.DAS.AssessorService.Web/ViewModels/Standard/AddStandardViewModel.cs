using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.Standard
{
    public class AddStandardViewModel
    {
        public Guid Id { get; set; }

        public string StandardToFind { get; set; }

        public string StandardReference { get; set; }

        public List<StandardVersion> Results { get; set; }

        public StandardVersion SelectedStandard { get; set; }

        public List<string> SelectedVersions { get; set; }

        public bool IsConfirmed { get; set; }

        public string ApplicationStatus { get; set; }

        public bool FromStandardsVersion { get; set; }

        public DateTime? EarliestVersionEffectiveFrom { get; set; }
    }
}
