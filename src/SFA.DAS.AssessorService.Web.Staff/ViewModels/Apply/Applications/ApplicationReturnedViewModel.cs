using System;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Apply.Applications
{
    public class ApplicationReturnedViewModel
    {
        public Guid ApplicationId { get; }
        public int SequenceId { get; }

        public ApplicationReturnedViewModel(Guid applicationId, int sequenceId)
        {
            ApplicationId = applicationId;
            SequenceId = sequenceId;
        }
    }
}
