using System.Collections.Generic;
using System.Text.RegularExpressions;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Validation
{
    public class RegexValidator : IValidator
    {
        public AssessorService.Api.Types.Models.Apply.ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            var regex = new Regex(ValidationDefinition.Value.ToString());
            return !regex.IsMatch(answer.Value)
                ? new List<KeyValuePair<string, string>>
                    {new KeyValuePair<string, string>(answer.QuestionId, ValidationDefinition.ErrorMessage)}
                : new List<KeyValuePair<string, string>>();
        }
    }
}