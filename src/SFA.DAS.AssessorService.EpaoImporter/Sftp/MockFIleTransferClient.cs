using System.IO;
using System.Threading.Tasks;
using Renci.SshNet;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.EpaoImporter.Sftp
{
    public class MockFileTransferClient : IFileTransferClient
    {
        private readonly SftpClient _sftpClient;
        private readonly IAggregateLogger _logger;
        private readonly IWebConfiguration _webConfiguration;

        public MockFileTransferClient(SftpClient sftpClient,
            IAggregateLogger logger,
            IWebConfiguration webConfiguration)
        {
            _sftpClient = sftpClient;
            _logger = logger;
            _webConfiguration = webConfiguration;
        }

        public async Task Send(MemoryStream memoryStream, string fileName)
        { 
            _logger.LogInfo("Sending file - {fileName} through sftp");
        }
    }
}
