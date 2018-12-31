using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class Application : ApplyTypeBase
    {
        public Organisation ApplyingOrganisation { get; set; }
        public Guid ApplyingOrganisationId { get; set; }
        public DateTime WithdrawnAt { get; set; }
        public string WithdrawnBy { get; set; }
        public string ApplicationStatus { get; set; }
        public List<ApplicationSequence> Sequences { get; set; }
    }
    
    public class ApplicationSequence : ApplyTypeBase
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public int SequenceId { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public List<ApplicationSection> Sections { get; set; }

        public bool HasNewFeedback
        {
            get
            {
                return Sections.SelectMany(s => s.QnADataObject.Pages).Any(p => p.HasFeedback && p.Feedback.Any(f => !f.IsCompleted));
            }
        }
    }

    public class ApplicationSequenceStatus
    {
        public const string Draft = "Draft";
        public const string Submitted = "Submitted";
    }
    
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

    public class DisplayAnswerPage
    {
        public List<DisplayAnswer> DisplayAnswers { get; set; }
    }

    public class DisplayAnswer
    {
        public string Label { get; set; }
        public string Answer { get; set; }
    }


    public class Question
    {
        public string QuestionId { get; set; }
        public string Label { get; set; }
        public string Hint { get; set; }
        public Input Input { get; set; }
        public int? Order { get; set; }
        public string PageId { get; set; }
    }
    
    public class Feedback
    {
        public string From { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsNew { get; set; }
    }
    
    public class PageOfAnswers
    {
        public Guid Id { get; set; }
        public List<Answer> Answers { get; set; }
    }
    
    public class Next
    {
        public string Action { get; set; }
        public string ReturnId { get; set; }
        public Condition Condition { get; set; }
        public bool ConditionMet { get; set; }
    }
    
    public class Input
    {
        public string Type { get; set; }
        public List<InputOptions> Options { get; set; }
        public List<ValidationDefinition> Validations { get; set; }
    }

    public class InputOptions
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public List<Question> FurtherQuestions { get; set; }
    }

    public class Answer
    {
        public string QuestionId { get; set; }
        public string Value { get; set; }
    }
    
    public class Condition
    {
        public string QuestionId { get; set; }
        public string MustEqual { get; set; }
    }
    
    public class ValidationDefinition
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public string ErrorMessage { get; set; }
    }
}


// 