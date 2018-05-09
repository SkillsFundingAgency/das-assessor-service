using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SFA.DAS.AssessorService.EpaoImporter.InfrastructureServices
{
    public interface IBlobContainerHelper
    {
        Task<CloudBlobContainer> GetContainer(string containerName);
    }
}