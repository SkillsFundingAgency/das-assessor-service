using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Validation
{
    public interface IValidatorFactory
    {
        List<IValidator> Build(Question question);
    }
}