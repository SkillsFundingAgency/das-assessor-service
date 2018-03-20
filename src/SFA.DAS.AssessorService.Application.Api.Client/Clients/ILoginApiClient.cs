using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface ILoginApiClient
    {
        Task<LoginResponse> Login(LoginRequest searchQuery);
    }
}