using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Validation
{
    public interface IValidator
    {
        AssessorService.Api.Types.Models.Apply.ValidationDefinition ValidationDefinition { get; set; }
        List<KeyValuePair<string, string>> Validate(Question question, Answer answer);
    }
}