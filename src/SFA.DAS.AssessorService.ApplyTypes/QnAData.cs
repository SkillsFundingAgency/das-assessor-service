using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class QnAData
    {
        public List<Page> Pages { get; set; }
        public FinancialApplicationGrade FinancialApplicationGrade { get; set; }

        public List<Feedback> Feedback { get; set; } // Section level feedback

        public bool HasFeedback => Feedback?.Any() ?? false;

        public bool HasNewFeedback => HasFeedback && Feedback.Any(f => f.IsNew || !f.IsCompleted);

        public bool HasCompletedFeedback => HasFeedback && Feedback.Any(f => f.IsCompleted);
    }
}