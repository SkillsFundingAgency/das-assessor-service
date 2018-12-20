using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private readonly Object _lock = new Object();

        public FileTransferClient(
            IAggregateLogger aggregateLogger,
            IWebConfiguration webConfiguration)
        {
            _aggregateLogger = aggregateLogger;
            _webConfiguration = webConfiguration;
        }

        public void Send(MemoryStream memoryStream, string fileName)
        {
            _fileName = fileName;
            _aggregateLogger.LogInfo($"Connection = {_webConfiguration.Sftp.RemoteHost}");
            _aggregateLogger.LogInfo($"Port = {_webConfiguration.Sftp.Port}");
            _aggregateLogger.LogInfo($"Username = {_webConfiguration.Sftp.Username}");
            _aggregateLogger.LogInfo($"Upload Directory = {_webConfiguration.Sftp.UploadDirectory}");
            _aggregateLogger.LogInfo($"Proof Directory = {_webConfiguration.Sftp.ProofDirectory}");
            _aggregateLogger.LogInfo($"FileName = { _webConfiguration.Sftp.UploadDirectory}/{fileName}");

            lock (_lock)
            {
                using (var sftpClient = new SftpClient(_webConfiguration.Sftp.RemoteHost,
                    Convert.ToInt32(_webConfiguration.Sftp.Port),
                    _webConfiguration.Sftp.Username,
                    _webConfiguration.Sftp.Password))
                {
                    sftpClient.Connect();

                    memoryStream.Position = 0; // ensure memory stream is set to begining of stream          

                    _aggregateLogger.LogInfo($"Uploading file ... {_webConfiguration.Sftp.UploadDirectory}/{fileName}");
                     sftpClient.UploadFile(memoryStream, $"{_webConfiguration.Sftp.UploadDirectory}/{fileName}",
                        UploadCallBack);

                    _aggregateLogger.LogInfo(
                        $"Validating Upload length of file ... {_webConfiguration.Sftp.UploadDirectory}/{fileName} = {memoryStream.Length}");
                    ValidateUpload(sftpClient, fileName, memoryStream.Length);

                    _aggregateLogger.LogInfo($"Validated the upload ...");
                }
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

        public async Task<List<string>> GetListOfDownloadedFiles()
        {
            using (var sftpClient = new SftpClient(_webConfiguration.Sftp.RemoteHost,
                Convert.ToInt32(_webConfiguration.Sftp.Port),
                _webConfiguration.Sftp.Username,
                _webConfiguration.Sftp.Password))
            {
                sftpClient.Connect();

                var fileList = await sftpClient.ListDirectoryAsync($"{_webConfiguration.Sftp.ProofDirectory}");
                return fileList.Where(f => !f.IsDirectory).Select(file => file.Name).ToList();
            }
        }

        private void UploadCallBack(ulong uploaded)
        {
            _aggregateLogger.LogInfo($"Uploading file progress ... {_webConfiguration.Sftp.UploadDirectory}/{_fileName} : {uploaded}");
        }

        private void ValidateUpload(SftpClient sftpClient, string fileName, long length)
        {
            using (var memoryStreamBack = new MemoryStream())
            {
                sftpClient.DownloadFile($"{_webConfiguration.Sftp.UploadDirectory}/{fileName}",
                    memoryStreamBack);
                memoryStreamBack.Position = 0;

                if (memoryStreamBack.Length != length)
                {
                    _aggregateLogger.LogInfo($"There has been  problem with the sftp file transfer with file name {_webConfiguration.Sftp.UploadDirectory}/{fileName}");
                    throw new ApplicationException(
                        $"There has been  problem with the sftp file transfer with file name {_webConfiguration.Sftp.UploadDirectory}/{fileName}");
                }
            }
        }
    }
}
