using System.IO;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype.Utilities
{
    public class FilePerister
    {
        private readonly IConfiguration _configuration;

        public FilePerister(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SaveCopy(string fileName, MemoryStream memoryStream)
        {
            var outputDirectory = _configuration["OutputDirectory"];
            var fullFileName = $"{outputDirectory}\\{fileName}";

            var file = new FileStream(fullFileName, FileMode.Create, FileAccess.Write);
            memoryStream.WriteTo(file);
            file.Close();

            LaunchDocument(fullFileName);
        }


        private void LaunchDocument(string fileName)
        {
            var launchDocument = _configuration["LaunchDocument"];
            if (launchDocument == "true")
                System.Diagnostics.Process.Start(fileName);
        }
    }
}
