using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Api.Common
{
    public interface ITokenService
    {
        Task<string> GetTokenAsync();
    }
}