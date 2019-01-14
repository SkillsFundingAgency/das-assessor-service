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

        public PageViewModel(Page page)
        {
            Page = page;
            Title = page.Title;
            ApplicationId = page.ApplicationId;
            SequenceId = page.SequenceId;
            SectionId = page.SectionId;
            PageId = page.PageId;
        }
    }
}
