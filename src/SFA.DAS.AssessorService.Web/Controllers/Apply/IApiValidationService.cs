using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.AssessorService.Web.Controllers.Apply
{
    public interface IApiValidationService
    {
        Task<ApiValidationResult> CallApiValidation(Guid Id, Page page, List<Answer> answers);
    }
}