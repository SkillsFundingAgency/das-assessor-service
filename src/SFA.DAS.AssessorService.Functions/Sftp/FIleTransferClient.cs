using System.IO;
using System.Threading.Tasks;
using Renci.SshNet;
using Renci.SshNet.Async;

namespace SFA.DAS.AssessorService.Functions.Sftp
{
    public class FileTransferClient
    {
        private readonly SftpClient _sftpClient;

        public FileTransferClient(SftpClient sftpClient)
        {
            _sftpClient = sftpClient;
        }

        public async Task Send(MemoryStream memoryStream, string fileName)
        {
            _sftpClient.Connect();

            memoryStream.Position = 0; // ensure memory stream is set to begining of stream
            await _sftpClient.UploadAsync(memoryStream, $"/upload/{fileName}");
            _sftpClient.Disconnect();
        }
    }
}
