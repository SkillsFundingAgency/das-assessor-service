using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Apply.Applications
{
    public class ApplicationSectionViewModel
    {
        public ApplicationSection Section { get; }

        public string Title { get; }

        public Guid ApplicationId { get; }

        public int SequenceId { get; }

        public int SectionId { get; }

        public bool AddFeedbackMessage { get; set; }
        public string FeedbackMessage { get; set; }
        public bool IsSectionComplete { get; set; }

        public ApplicationSectionViewModel(ApplicationSection section)
        {
            Section = section;
            Title = section.Title;
            ApplicationId = section.ApplicationId;
            SequenceId = section.SequenceId;
            SectionId = section.SectionId;

            FeedbackMessage = section.QnAData?.Feedback?.Where(f => f.IsNew).Select(f => f.Message).FirstOrDefault();
            AddFeedbackMessage = !string.IsNullOrWhiteSpace(FeedbackMessage);
            IsSectionComplete = section.Status == ApplicationSectionStatus.Evaluated;
        }
    }
}
