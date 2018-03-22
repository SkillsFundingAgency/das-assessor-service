using System.IO;
using System.Threading.Tasks;
using Renci.SshNet;
using Renci.SshNet.Async;
using SFA.DAS.AssessorService.DocumentConversion.Prototype.Utilities;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype.Sftp
{
    public class FileTransferClient
    {
        private readonly SftpClient _sftpClient;
        private readonly FilePerister _filePerister;

        public FileTransferClient(SftpClient sftpClient,
            FilePerister filePerister)
        {
            _sftpClient = sftpClient;
            _filePerister = filePerister;
        }

        public async Task Send(MemoryStream memoryStream, string fileName)
        {
            _sftpClient.Connect();

            memoryStream.Position = 0; // ensure memory stream is set to begining of stream
            await _sftpClient.UploadAsync(memoryStream, $"/upload/{fileName}");
            _sftpClient.Disconnect();

            _filePerister.SaveCopy(fileName, memoryStream);
        }
    }
}
