using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Domain.Extensions;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.Standard
{
    public class AddStandardConfirmViewModel
    {
        public string Search { get; set; }
        
        public string StandardReference { get; set; }

        public StandardVersion Standard { get; set; }

        public List<StandardVersion> StandardVersions { get; set; }

        public List<string> SelectedVersions { get; set; }

        public bool IsConfirmed { get; set; }

        public string StandardTitle => Standard?.Title;

        public string StandardEffectiveFrom => Standard?.EffectiveFrom?.ToSfaShortDateString();

        public string StandardEffectiveTo => Standard?.EffectiveTo?.ToSfaShortDateString();

        public string SelectedVersionsText => $"Version{(SelectedVersions?.Count > 1 ? "s" : string.Empty)}";

        public string SelectedVersionsValue => SelectedVersions != null ? string.Join(", ", SelectedVersions) : string.Empty;
    }
}
