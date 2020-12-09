using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface ISandboxDbRepository
    {
        Task RebuildExternalApiSandbox();
    }
}
