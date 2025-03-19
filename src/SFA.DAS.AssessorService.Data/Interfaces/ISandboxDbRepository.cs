using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.Interfaces
{
    public interface ISandboxDbRepository
    {
        Task RebuildExternalApiSandbox();
    }
}
