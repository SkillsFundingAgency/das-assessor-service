using SFA.DAS.AssessorService.Api.Types.Models;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface ILoginApiClient
    {
        Task<LoginResponse> Login(LoginRequest searchQuery);
    }
}