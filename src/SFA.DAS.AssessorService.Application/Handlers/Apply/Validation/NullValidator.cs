using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Validation
{
    public class NullValidator : IValidator
    {
        public AssessorService.Api.Types.Models.Apply.ValidationDefinition ValidationDefinition { get; set; }
        public List<KeyValuePair<string, string>> Validate(Question question, Answer answer)
        {
            return new List<KeyValuePair<string, string>>();
        }
    }
}