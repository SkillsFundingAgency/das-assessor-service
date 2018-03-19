using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Web.Orchestrators.Login
{
    public interface ILoginOrchestrator
    {
        Task<LoginResponse> Login();
    }
}