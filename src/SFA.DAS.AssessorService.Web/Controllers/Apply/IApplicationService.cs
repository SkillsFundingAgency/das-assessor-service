using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Controllers.Apply
{
    public interface IApplicationService
    {
        Task<bool> ResetApplicationToStage1(Guid id, Guid userId);
    }
}
