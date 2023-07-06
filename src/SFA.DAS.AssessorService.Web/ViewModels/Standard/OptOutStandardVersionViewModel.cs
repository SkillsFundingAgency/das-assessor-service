using System;

namespace SFA.DAS.AssessorService.Web.ViewModels.Standard
{
    public class OptOutStandardVersionViewModel
    {
        public Guid Id { get; set; }
        public string StandardReference { get; set; }
        public string StandardTitle { get; set; }
        public string Version { get; set; }

        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
