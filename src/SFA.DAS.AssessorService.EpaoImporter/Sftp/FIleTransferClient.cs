using System;
using System.IO;
using System.Threading.Tasks;
using Renci.SshNet;
using Renci.SshNet.Async;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.EpaoImporter.Sftp
{
    public class FileTransferClient : IFileTransferClient
    {
        private readonly SftpClient _sftpClient;
        private readonly IAggregateLogger _aggregateLogger;
        private readonly IWebConfiguration _webConfiguration;

        public FileTransferClient(SftpClient sftpClient,
            IAggregateLogger aggregateLogger,
            IWebConfiguration webConfiguration)
        {
            _sftpClient = sftpClient;
            _aggregateLogger = aggregateLogger;
            _webConfiguration = webConfiguration;
        }

        public async Task Send(MemoryStream memoryStream, string fileName)
        {
            _aggregateLogger.LogInfo($"Connection = {_webConfiguration.Sftp.RemoteHost}");
            _aggregateLogger.LogInfo($"Port = {_webConfiguration.Sftp.Port}");
            _aggregateLogger.LogInfo($"Username = {_webConfiguration.Sftp.Username}");
            _aggregateLogger.LogInfo($"Upload Directory = {_webConfiguration.Sftp.UploadDirectory}");
            _aggregateLogger.LogInfo($"Proof Directory = {_webConfiguration.Sftp.ProofDirectory}");
            _aggregateLogger.LogInfo($"FileName = { _webConfiguration.Sftp.UploadDirectory}/{fileName}");

            _sftpClient.Connect();

            memoryStream.Position = 0; // ensure memory stream is set to begining of stream          

            await _sftpClient.UploadAsync(memoryStream, $"{_webConfiguration.Sftp.UploadDirectory}/{fileName}");

            await ValidateUpload(fileName, memoryStream.Length);

            _sftpClient.Disconnect();
        }

        private async Task ValidateUpload(string fileName, long length)
        {
            var memoryStreamBack = new MemoryStream();
            await _sftpClient.DownloadAsync($"{_webConfiguration.Sftp.UploadDirectory}/{fileName}", memoryStreamBack);
            memoryStreamBack.Position = 0;

            if(memoryStreamBack.Length != length)
                 throw new ApplicationException($"There has been  problem with the sftp file transfer with file name {_webConfiguration.Sftp.UploadDirectory}/{fileName}");
        }
    }
}
    