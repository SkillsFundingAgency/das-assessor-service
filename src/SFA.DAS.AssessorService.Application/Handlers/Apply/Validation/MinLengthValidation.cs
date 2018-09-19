using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Validation
{
    public class MinLengthValidation : IValidator
    {
        public AssessorService.Api.Types.Models.Apply.ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            if (answer.Value.Length < (long)ValidationDefinition.Value)
            {
                return new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(answer.QuestionId,
                        ValidationDefinition.ErrorMessage)
                };
            }
            return new List<KeyValuePair<string, string>>();
        }
    }
}