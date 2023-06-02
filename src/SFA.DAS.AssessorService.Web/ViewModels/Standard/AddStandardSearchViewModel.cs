using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.Standard
{
    public class AddStandardSearchViewModel
    {
        public string StandardToFind { get; set; }

        public List<StandardVersion> Results { get; set; }

        public List<StandardVersion> Approved { get; set; }

        public List<string> SelectedVersions { get; set; }

        public bool IsConfirmed { get; set; }

        public bool IsApprovedForStandard(StandardVersion standardVersion)
        {
            return Approved.Exists(p => p.IFateReferenceNumber == standardVersion.IFateReferenceNumber);
        }
    }
}
