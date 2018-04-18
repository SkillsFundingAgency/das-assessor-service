using System.IO;
using System.Threading.Tasks;
using Renci.SshNet;
using Renci.SshNet.Async;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.EpaoImporter.Sftp
{
    public class FileTransferClient : IFileTransferClient
    {
        private readonly SftpClient _sftpClient;
        private readonly IWebConfiguration _webConfiguration;

        public FileTransferClient(SftpClient sftpClient,
            IWebConfiguration webConfiguration)
        {
            _sftpClient = sftpClient;
            _webConfiguration = webConfiguration;
        }

        public async Task Send(MemoryStream memoryStream, string fileName)
        {
            _sftpClient.Connect();

            memoryStream.Position = 0; // ensure memory stream is set to begining of stream
          
            await _sftpClient.UploadAsync(memoryStream, $"{_webConfiguration.Sftp.UploadDirectory}/{fileName}");
            _sftpClient.Disconnect();
        }
    }
}
