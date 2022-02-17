using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Standards
{
    public class StandardVersionApplication : StandardVersion
    {
        public string VersionStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public Guid ApplicationId { get; set; }
        public bool PreviouslyWithdrawn { get; set; }
        public StandardVersionApplication(AppliedStandardVersion standard)
        {
            StandardUId = standard.StandardUId;
            Title = standard.Title;
            Version = standard.Version;
            IFateReferenceNumber = standard.IFateReferenceNumber;
            LarsCode = standard.LarsCode;
            Level = standard.Level;
            EffectiveFrom = standard.LarsEffectiveFrom.GetValueOrDefault();
            EffectiveTo = standard.LarsEffectiveTo;
            EPAChanged = standard.EPAChanged;
            StandardPageUrl = standard.StandardPageUrl;
            ApplicationStatus = standard.ApplicationStatus;
            ApplicationId = standard.ApplicationId;
        }
    }
}
