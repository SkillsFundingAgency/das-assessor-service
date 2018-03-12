using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.AssessorService.Web.Orchestrators.Login
{
    public interface ILoginOrchestrator
    {
        Task<LoginResult> Login(HttpContext context);
    }
}