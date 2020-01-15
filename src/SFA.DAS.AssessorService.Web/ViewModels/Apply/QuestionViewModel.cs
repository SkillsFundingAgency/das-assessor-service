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
        public string InputPrefix { get; set; }
        public string InputSuffix { get; set; }
        public string Value { get; set; }
        public dynamic JsonValue { get; set; }
        public List<Option> Options { get; set; }
        public List<ValidationDefinition> Validations { get; set; }
        public List<ValidationErrorDetail> ErrorMessages { get; set; }

        public Guid Id { get; set; }
        public int SequenceNo { get; set; }
        public int SectionNo { get; set; }
        public Guid SectionId { get; set; }
        public string PageId { get; set; }
        public string RedirectAction { get; set; }

        public string DisplayAnswerValue(Answer answer)
        {
            var answerValue = answer?.Value;

            if (!string.IsNullOrWhiteSpace(answer?.Value))
            {
                var dateparts = answer.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                if ("Date".Equals(Type, StringComparison.InvariantCultureIgnoreCase) && dateparts.Length == 3)
                {
                    if (DateTime.TryParse($"{dateparts[0]}/{dateparts[1]}/{dateparts[2]}", out var datetime))
                    {
                        answerValue = datetime.ToString("dd/MM/yyyy");
                    }
                }
                else if ("MonthAndYear".Equals(Type, StringComparison.InvariantCultureIgnoreCase) && dateparts.Length == 2)
                {
                    if (DateTime.TryParse($"{dateparts[0]}/{dateparts[1]}", out var datetime))
                    {
                        answerValue = datetime.ToString("MM/yyyy");
                    }
                }
            }

            return answerValue ?? string.Empty;
        }
    }
}