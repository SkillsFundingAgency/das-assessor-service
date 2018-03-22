using System;
using System.IO;
using System.Threading.Tasks;
using Renci.SshNet;
using Renci.SshNet.Async;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype.Sftp
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
            try
            {
                //var client = new SftpClient("localhost", 2222, "foo", "pass");
                _sftpClient.Connect();

                // await a directory listing
                var listing = await _sftpClient.ListDirectoryAsync(".");
              
                await _sftpClient.UploadAsync(memoryStream, $"/upload/{fileName}");
                _sftpClient.Disconnect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
