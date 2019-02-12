using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class Application : ApplyTypeBase
    {
        public Organisation ApplyingOrganisation { get; set; }
        public Guid ApplyingOrganisationId { get; set; }
        public DateTime WithdrawnAt { get; set; }
        public string WithdrawnBy { get; set; }
        public string ApplicationStatus { get; set; }
        public ApplicationData ApplicationData { get; set; }
    }

    public class ApplicationData
    {
        public string ReferenceNumber { get; set; }
        public int StandardCode { get; set; }
        public string StandardName { get; set; }
        public List<Submission> InitSubmissions { get; set; }
        public int InitSubmissionsCount { get; set; }
        public DateTime? LatestInitSubmissionDate { get; set; }
        public DateTime? InitSubmissionFeedbackAddedDate { get; set; }
        public DateTime? InitSubmissionClosedDate { get; set; }
        public List<Submission> StandardSubmissions { get; set; }
        public int StandardSubmissionsCount { get; set; }
        public DateTime? LatestStandardSubmissionDate { get; set; }
        public DateTime? StandardSubmissionFeedbackAddedDate { get; set; }
        public DateTime? StandardSubmissionClosedDate { get; set; }
    }

    public class Submission
    {
        public DateTime SubmittedAt { get; set; }
        public Guid SubmittedBy { get; set; }
        public string SubmittedByEmail { get; set; }
    }


    public class DisplayAnswerPage
    {
        public List<IDisplayAnswer> DisplayAnswers { get; set; }
    }

    public class DisplayAnswer : IDisplayAnswer
    {
        public string Label { get; set; }
        string IDisplayAnswer.Answer()
        {
            return Answer;
        }

        public string Answer { private get; set; }
    }

    public class FileUploadDisplayAnswer : IDisplayAnswer
    {
        public string Label { get; set; }
        public Guid ApplicationId { get; set; }
        public int SequenceId { get; set; }
        public int SectionId { get; set; }
        public string PageId { get; set; }
        public string QuestionId { get; set; }
        public string FileName { get; set; }
        public string Answer()
        {
            return "";
        }
    }
    
    public interface IDisplayAnswer
    {
        string Label { get; set; }
        string Answer();
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
        public Guid Id { get; set; }
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