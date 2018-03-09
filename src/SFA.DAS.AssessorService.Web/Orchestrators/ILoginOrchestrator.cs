using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.AssessorService.Web.Orchestrators
{
    public interface ILoginOrchestrator
    {
        Task<LoginResult> Login(HttpContext context);
    }
}