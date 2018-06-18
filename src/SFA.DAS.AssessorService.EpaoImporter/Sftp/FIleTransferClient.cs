using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using Renci.SshNet.Async;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.EpaoImporter.Sftp
{
    public class FileTransferClient : IFileTransferClient
    {        
        private readonly IAggregateLogger _aggregateLogger;
        private readonly IWebConfiguration _webConfiguration;
        private string _fileName;

        public FileTransferClient(
            IAggregateLogger aggregateLogger,
            IWebConfiguration webConfiguration)
        {        
            _aggregateLogger = aggregateLogger;
            _webConfiguration = webConfiguration;
        }

        public async Task Send(MemoryStream memoryStream, string fileName)
        {
            _fileName = fileName;
            _aggregateLogger.LogInfo($"Connection = {_webConfiguration.Sftp.RemoteHost}");
            _aggregateLogger.LogInfo($"Port = {_webConfiguration.Sftp.Port}");
            _aggregateLogger.LogInfo($"Username = {_webConfiguration.Sftp.Username}");
            _aggregateLogger.LogInfo($"Upload Directory = {_webConfiguration.Sftp.UploadDirectory}");
            _aggregateLogger.LogInfo($"Proof Directory = {_webConfiguration.Sftp.ProofDirectory}");
            _aggregateLogger.LogInfo($"FileName = { _webConfiguration.Sftp.UploadDirectory}/{fileName}");

            using (var sftpClient = new SftpClient(_webConfiguration.Sftp.RemoteHost,
                Convert.ToInt32(_webConfiguration.Sftp.Port),
                _webConfiguration.Sftp.Username,
                _webConfiguration.Sftp.Password))
            {
                sftpClient.Connect();

                memoryStream.Position = 0; // ensure memory stream is set to begining of stream          

                _aggregateLogger.LogInfo($"Uploading file ... {_webConfiguration.Sftp.UploadDirectory}/{fileName}");
                await sftpClient.UploadAsync(memoryStream, $"{_webConfiguration.Sftp.UploadDirectory}/{fileName}", UploadCallBack);

                _aggregateLogger.LogInfo($"Validating Upload length of file ... {_webConfiguration.Sftp.UploadDirectory}/{fileName} = {memoryStream.Length}");
                await ValidateUpload(sftpClient, fileName, memoryStream.Length);           
            }
        }

        public async Task LogUploadDirectory()
        {
            using (var sftpClient = new SftpClient(_webConfiguration.Sftp.RemoteHost,
                Convert.ToInt32(_webConfiguration.Sftp.Port),
                _webConfiguration.Sftp.Username,
                _webConfiguration.Sftp.Password))
            {
                sftpClient.Connect();

                var fileList = await sftpClient.ListDirectoryAsync($"{_webConfiguration.Sftp.UploadDirectory}");
                var fileDetails = new StringBuilder();
                foreach (var file in fileList)
                {
                    fileDetails.Append(file + "\r\n");
                }

                if (fileDetails.Length > 0)
                    _aggregateLogger.LogInfo(
                        $"Uploaded Files to {_webConfiguration.Sftp.UploadDirectory} Contains\r\n{fileDetails}");
            }
        }

        private void UploadCallBack(ulong uploaded)
        {
            _aggregateLogger.LogInfo($"Uploading file progress ... {_webConfiguration.Sftp.UploadDirectory}/{_fileName} : {uploaded}");
        }

        private async Task ValidateUpload(SftpClient sftpClient, string fileName, long length)
        {
            using (var memoryStreamBack = new MemoryStream())
            {
                await sftpClient.DownloadAsync($"{_webConfiguration.Sftp.UploadDirectory}/{fileName}",
                    memoryStreamBack);
                memoryStreamBack.Position = 0;

                if (memoryStreamBack.Length != length)
                    throw new ApplicationException(
                        $"There has been  problem with the sftp file transfer with file name {_webConfiguration.Sftp.UploadDirectory}/{fileName}");
            }
        }
    }
}
