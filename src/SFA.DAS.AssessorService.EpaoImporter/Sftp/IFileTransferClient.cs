using System.IO;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.EpaoImporter.Sftp
{
    public interface IFileTransferClient
    {
        void Send(MemoryStream memoryStream, string fileName);
        Task LogUploadDirectory();
    }
}