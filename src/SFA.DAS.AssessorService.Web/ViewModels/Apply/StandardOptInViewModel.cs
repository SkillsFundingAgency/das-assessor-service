using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.Apply
{
    public class StandardOptInViewModel
    {
        public Guid Id { get; set; }
        public string StandardReference { get; set; }
        public string StandardTitle { get; set; }
        public string Version { get; set; }

        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
