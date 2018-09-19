using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Validation
{
    public class MaxLengthValidator : IValidator
    {
        public AssessorService.Api.Types.Models.Apply.ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            var errors = new List<KeyValuePair<string, string>>();
            if (answer.Value.Length > (long)ValidationDefinition.Value)
            {
                errors.Add(new KeyValuePair<string, string>(answer.QuestionId, ValidationDefinition.ErrorMessage));
            }

            return errors;
        }
    }
}