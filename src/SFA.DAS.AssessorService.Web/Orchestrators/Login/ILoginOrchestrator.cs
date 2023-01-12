using SFA.DAS.AssessorService.Api.Types.Models;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Orchestrators.Login
{
    public interface ILoginOrchestrator
    {
        Task<LoginResponse> Login();
    }
}