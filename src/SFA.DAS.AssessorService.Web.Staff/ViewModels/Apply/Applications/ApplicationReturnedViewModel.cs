using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Apply.Applications
{
    public class ApplicationReturnedViewModel
    {
        public Guid ApplicationId { get; }
        public int SequenceId { get; }

        public List<string> WarningMessages { get; set; }

        public ApplicationReturnedViewModel(Guid applicationId, int sequenceId, List<string> warningMessages)
        {
            ApplicationId = applicationId;
            SequenceId = sequenceId;
            WarningMessages = warningMessages;
        }
    }
}
