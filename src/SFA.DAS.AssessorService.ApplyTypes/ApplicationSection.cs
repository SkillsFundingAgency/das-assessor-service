using SFA.DAS.QnA.Api.Types;
using System.Linq;

namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class ApplicationSection : ApplyTypeBase
    {
        public Section Section { get; set; }
        public int SequenceNo { get; set; }
        public string PageContext { get; set; }

        public bool HasNewPageFeedback => Section.QnAData.Pages.Any(p => p.HasNewFeedback);
        public bool HasCompletedPageFeedback => Section.QnAData.Pages.Any(p => p.AllFeedbackIsCompleted);
    }

    public static class ApplicationSectionStatus
    {
        public const string Draft = "Draft";
        public const string Submitted = "Submitted";
        public const string InProgress = "In Progress";
        public const string Graded = "Graded";
        public const string Evaluated = "Evaluated";
    }

    public static class SectionDisplayType
    {
        public const string Pages = "Pages";
        public const string Questions = "Questions";
        public const string PagesWithSections = "PagesWithSections";
    }
}
