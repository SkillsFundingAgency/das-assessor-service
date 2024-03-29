﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.QnA.Api.Types.Page;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.ViewModels.Apply
{
    public class PageViewModel
    {
        public Guid Id { get; }

        public PageViewModel(Guid id, int sequenceNo, int sectionNo, string pageId, Page page, string pageContext, string redirectAction, string returnUrl, List<ValidationErrorDetail> errorMessages, string summaryLink = "Show")
        {
            Id = id;
            SequenceNo = sequenceNo;
            SectionNo = sectionNo;
            PageId = pageId;
            PageContext = pageContext;
            RedirectAction = redirectAction;
            ReturnUrl = returnUrl;
            ErrorMessages = errorMessages;
            SummaryLink = summaryLink;

            if (page != null)
            {
                SetupPage(page, errorMessages);
            }
        }

        public bool HasFeedback { get; set; }
        public List<Feedback> Feedback { get; set; }

        public string LinkTitle { get; set; }

        public string PageId { get; set; }
        public string PageContext { get; set; }
        public string Title { get; set; }

        public string DisplayType { get; set; }

        public List<QuestionViewModel> Questions { get; set; }

        public int SequenceNo { get; set; }
        public int SectionNo { get; set; }
        public Guid SectionId { get; set; }

        public bool AllowMultipleAnswers { get; set; }
        public List<PageOfAnswers> PageOfAnswers { get; set; }
        public string BodyText { get; set; }

        public PageDetails Details { get; set; }

        public string RedirectAction { get; set; }
        public string ReturnUrl { get; set; }
        
        public string SummaryLink { get; set; }

        public List<ValidationErrorDetail> ErrorMessages { get; set; }

        private void SetupPage(Page page, List<ValidationErrorDetail> errorMessages)
        {
            Title = page.Title;
            LinkTitle = page.LinkTitle;
            DisplayType = page.DisplayType;
            PageId = page.PageId;
            SectionId = page.SectionId ?? Guid.Empty;

            Feedback = page.Feedback;
            HasFeedback = page.HasFeedback;

            BodyText = page.BodyText;
            Details = page.Details;

            AllowMultipleAnswers = page.AllowMultipleAnswers;
            PageOfAnswers = page.PageOfAnswers ?? new List<PageOfAnswers>();

            // MultipleAnswer questions stores the last failed attempt as a previous answer so it needs to be removed
            if (AllowMultipleAnswers && errorMessages != null && errorMessages.Any())
            {
                PageOfAnswers = page.PageOfAnswers.Take(page.PageOfAnswers.Count - 1).ToList();
            }

            var answers = new List<Answer>();

            // Grab the latest answer for each question stored within the page
            foreach (var pageAnswer in page.PageOfAnswers.SelectMany(poa => poa.Answers))
            {
                var currentAnswer = answers.FirstOrDefault(a => a.QuestionId == pageAnswer.QuestionId);
                if (currentAnswer is null)
                {
                    answers.Add(new Answer() { QuestionId = pageAnswer.QuestionId, Value = pageAnswer.Value });
                }
                else
                {
                    currentAnswer.Value = pageAnswer.Value;
                }
            }

            Questions = new List<QuestionViewModel>();
            Questions.AddRange(page.Questions.Select(q => new QuestionViewModel()
            {
                Label = q.Label,
                ShortLabel = q.ShortLabel,
                QuestionBodyText = q.QuestionBodyText,
                QuestionId = q.QuestionId,
                Type = q.Input.Type,
                InputClasses = q.Input.InputClasses,
                InputPrefix = q.Input.InputPrefix,
                InputSuffix = q.Input.InputSuffix,
                Hint = q.Hint,
                Options = q.Input.Options,
                Validations = q.Input.Validations,
                Value = page.AllowMultipleAnswers ? GetMultipleValue(page.PageOfAnswers.LastOrDefault()?.Answers, q, errorMessages) : answers?.SingleOrDefault(a => a?.QuestionId == q.QuestionId)?.Value,
                JsonValue = page.AllowMultipleAnswers ? GetMultipleJsonValue(page.PageOfAnswers.LastOrDefault()?.Answers, q, errorMessages) : GetJsonValue(answers,q),
                ErrorMessages = errorMessages?.Where(f => f.Field.Split("_Key_")[0] == q.QuestionId).ToList(),
                SequenceNo = SequenceNo,
                SectionNo = SectionNo,
                SectionId = SectionId,
                Id = Id,
                PageId = PageId,
                RedirectAction = RedirectAction
            }));

            foreach (var question in Questions)
            {
                if (question.Options == null) continue;
                foreach (var option in question.Options)
                {
                    if (option.FurtherQuestions == null) continue;
                    foreach (var furtherQuestion in option.FurtherQuestions)
                    {
                        furtherQuestion.Value = answers
                            ?.SingleOrDefault(a => a?.QuestionId == furtherQuestion.QuestionId.ToString())?.Value;
                    }
                }
            }
        }

        private string GetMultipleValue(List<Answer> answers, Question question, List<ValidationErrorDetail> errorMessages)
        {
            if (errorMessages != null && errorMessages.Any())
            {
                return answers?.LastOrDefault(a => a?.QuestionId == question.QuestionId)?.Value;
            }

            return null;
        }

        private dynamic GetMultipleJsonValue(List<Answer> answers, Question question, List<ValidationErrorDetail> errorMessages)
        {
            if (errorMessages != null && errorMessages.Any())
            {
                return JsonConvert.SerializeObject(answers?.LastOrDefault(a => a?.QuestionId == question.QuestionId)?.Value);
            }

            return null;
        }

        private dynamic GetJsonValue(List<Answer> answers, Question question)
        {
            var json = answers?.SingleOrDefault(a => a?.QuestionId == question.QuestionId)?.Value;
            try
            {
                JToken.Parse(json);
                return JsonConvert.DeserializeObject<dynamic>(json);

            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}