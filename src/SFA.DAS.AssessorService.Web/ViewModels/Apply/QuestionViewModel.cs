using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.QnA.Api.Types.Page;
using System;
using System.Collections.Generic;
namespace SFA.DAS.AssessorService.Web.ViewModels.Apply
{
    public class QuestionViewModel
    {
        public string QuestionId { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
        public string ShortLabel { get; set; }
        public string QuestionBodyText { get; set; }
        public string Hint { get; set; }
        public string InputClasses { get; set; }
        public string Value { get; set; }
        public List<Option> Options { get; set; }
        public List<ValidationErrorDetail> ErrorMessages { get; set; }

        public Guid Id { get; set; }
        public int SequenceNo { get; set; }
        public Guid SectionId { get; set; }
        public string PageId { get; set; }
        public string RedirectAction { get; set; }

        public string DisplayAnswerValue(Answer answer)
        {
            if (Type == "Date")
            {
                var dateparts = answer.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                var day = dateparts[0];
                var month = dateparts[1];
                var year = dateparts[2];

                var datetime = DateTime.Parse($"{day}/{month}/{year}");
                return datetime.ToString("dd/MM/yyyy");
            }
            return answer.Value;
        }
    }
}