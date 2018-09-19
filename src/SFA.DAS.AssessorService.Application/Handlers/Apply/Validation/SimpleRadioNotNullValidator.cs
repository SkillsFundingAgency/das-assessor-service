using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Validation
{
    public class SimpleRadioNotNullValidator : IValidator
    {
        public AssessorService.Api.Types.Models.Apply.ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            if (answer == null)
            {
                return new List<KeyValuePair<string, string>>()
                    {new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage)};
            }
            else
            {
                return new List<KeyValuePair<string, string>>();
            }
        }
    }
}