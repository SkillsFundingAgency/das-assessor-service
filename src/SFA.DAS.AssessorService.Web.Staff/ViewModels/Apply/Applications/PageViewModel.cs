using SFA.DAS.AssessorService.ApplyTypes;
using System;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Apply.Applications
{
    public class PageViewModel
    {
        public Page Page { get; }

        public string Title { get; }

        public Guid ApplicationId { get; }

        public int SequenceId { get; }

        public int SectionId { get; }

        public string PageId { get; }

        public string FeedbackMessage { get; set; }

        public PageViewModel(Guid applicationId, int sequenceId, int sectionId, string pageId, Page page)
        {
            if (page != null)
            {
                Page = page;
                Title = page.Title;
                ApplicationId = page.ApplicationId;
                SequenceId = page.SequenceId;
                SectionId = page.SectionId;
                PageId = page.PageId;
            }
            else
            {
                ApplicationId = applicationId;
                SequenceId = sequenceId;
                SectionId = sectionId;
                PageId = pageId;
            }
        }
    }
}
