using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System;

namespace SFA.DAS.AssessorService.Web.ViewModels.Standard
{
    public class ApplyStandardConfirmOfqualViewModel
    {
        public Guid Id { get; set; }

        public string Search { get; set; }

        public AppliedStandardVersion SelectedStandard { get; set; }
    }
}
