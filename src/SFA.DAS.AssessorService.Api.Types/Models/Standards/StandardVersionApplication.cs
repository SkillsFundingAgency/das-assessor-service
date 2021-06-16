using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Standards
{
    public class StandardVersionApplication : StandardVersion
    {
        public string VersionStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public Guid ApplicationId { get; set; }

        public StandardVersionApplication(AppliedStandardVersion standard)
        {
            StandardUId = standard.StandardUId;
            Title = standard.Title;
            Version = standard.Version.ToString();
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
