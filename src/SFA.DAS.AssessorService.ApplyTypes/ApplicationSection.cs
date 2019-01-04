using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class ApplicationSection : ApplyTypeBase
    {
        public Guid ApplicationId { get; set; }
        public int SectionId { get; set; }
        public int SequenceId { get; set; }
        public string FeedbackComment { get; set; }
        public string QnAData { get; set; }

        public QnAData QnADataObject
        {
            get => JsonConvert.DeserializeObject<QnAData>(QnAData);
            set => QnAData = JsonConvert.SerializeObject(value);
        }

        //        public List<Page> Pages
        //        {
        //            get => JsonConvert.DeserializeObject<List<Page>>(QnAData);
        //            set => QnAData = JsonConvert.SerializeObject(value);
        //        }

        public int PagesComplete
        {
            get { return QnADataObject.Pages.Count(p => p.Active && p.Complete); }
        }

        public int PagesActive
        {
            get { return QnADataObject.Pages.Count(p => p.Active); }
        }

        public bool HasNewFeedback
        {
            get
            {
                return QnADataObject.Pages.Any(p => p.HasFeedback && p.Feedback.Any(f => !f.IsCompleted));
            }
        }

        public bool HasReadFeedback
        {
            get
            {
                return QnADataObject.Pages.Any(p => p.HasFeedback && p.Feedback.Any(f => f.IsCompleted));
            }
        }

        public string Title { get; set; }
        public string LinkTitle { get; set; }
        public string DisplayType { get; set; }

        public List<Page> PagesContainingUploadQuestions
        {
            get { return QnADataObject.Pages.Where(p => p.ContainsUploadQuestions).ToList(); }
        }
    }

    public class ApplicationSectionStatus
    {
        public const string Submitted = "Submitted";
        public const string InProgress = "In Progress";
        public const string Graded = "Graded";
        public const string Evaluated = "Evaluated";
    }
}
