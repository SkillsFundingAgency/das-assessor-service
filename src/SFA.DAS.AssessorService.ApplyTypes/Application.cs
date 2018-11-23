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
        public string QnAData { get; set; }
        
        public List<Page> Pages
        {
            get => JsonConvert.DeserializeObject<List<Page>>(QnAData);
            set => QnAData = JsonConvert.SerializeObject(value);
        }

        public int PagesComplete
        {
            get { return Pages.Count(p => p.Active && p.Complete); }
        }
        
        public int PagesActive
        {
            get { return Pages.Count(p => p.Active); }
        }


        public string Title { get; set; }
        public string LinkTitle { get; set; }
        public string DisplayType { get; set; }
    }
    
    public class Page
    {
        public Guid ApplicationId { get; set; }
        private List<DisplayAnswerPage> _displayAnswerPages;
        public string PageId { get; set; }
        public string SequenceId { get; set; }
        public string SectionId { get; set; }
        public string Title { get; set; }
        public string LinkTitle { get; set; }
        public string InfoText { get; set; }
        public List<Question> Questions { get; set; }
        public List<PageOfAnswers> PageOfAnswers { get; set; }
        public List<Next> Next { get; set; }
        public bool Complete { get; set; }
        public bool AllowMultipleAnswers { get; set; }
        public int? Order { get; set; }
        public bool Active { get; set; }        
        public bool Visible { get; set; }
      
        public List<Feedback> Feedback { get; set; }
        public bool HasFeedback => Feedback?.Any() ?? false;
        
        public List<DisplayAnswerPage> DisplayAnswerPages
        {
            get
            {
                if (_displayAnswerPages != null) return _displayAnswerPages;
                
                _displayAnswerPages = new List<DisplayAnswerPage>();
                foreach (var answerPage in PageOfAnswers)
                {
                    var displayAnswerPage = new DisplayAnswerPage();
                    displayAnswerPage.DisplayAnswers = new List<DisplayAnswer>();
                        
                    foreach (var answer in answerPage.Answers)
                    {
                        var question = Questions.SingleOrDefault(q => q.QuestionId == answer.QuestionId);
                        if (question != null)
                        {
                            var displayAnswer = new DisplayAnswer
                            {
                                Answer = answer.Value,
                                Label = question.Label
                            };

                            displayAnswerPage.DisplayAnswers.Add(displayAnswer);   
                        }
                        else
                        {
                            // question is null, try embedded questions in ComplexRadio / Further Questions
                            question = Questions.Single(q => q.QuestionId == answer.QuestionId.Split(new[]{'.'},StringSplitOptions.RemoveEmptyEntries)[0]);

                            var furtherQuestion = question.Input.Options.Where(o => o.FurtherQuestions != null).SelectMany(o => o.FurtherQuestions).Single(q => q.QuestionId == answer.QuestionId);
                            
                            var displayAnswer = new DisplayAnswer
                            {
                                Answer = answer.Value,
                                Label = furtherQuestion.Label
                            };

                            displayAnswerPage.DisplayAnswers.Add(displayAnswer);   
                        }
                    }
                        
                    _displayAnswerPages.Add(displayAnswerPage);
                }
                return _displayAnswerPages;
            }
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
    }
    
    public class Feedback
    {
        public string From { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
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