using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SFA.DAS.AssessorService.DocumentConversion.Prototype.Services;
using SFA.DAS.AssessorService.DocumentConversion.Prototype.Utilities;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype
{
    public class Command
    {
        private readonly CoverLetterTemplateService _coverLetterTemplateService;
        private readonly IFACertificateService _ifaCertificateService;
        private readonly IConfiguration _configuration;
        private readonly FileUtilities _fileUtilities;

        public Command(CoverLetterTemplateService coverLetterTemplateService,
            IFACertificateService ifaCertificateService,
            IConfiguration configuration,
            FileUtilities fileUtilities)
        {
            _coverLetterTemplateService = coverLetterTemplateService;
            _ifaCertificateService = ifaCertificateService;
            _configuration = configuration;
            _fileUtilities = fileUtilities;
        }

        public async Task Execute()
        {
            CleanUpLastRun();

            await _coverLetterTemplateService.Create();
            await _ifaCertificateService.Create();
        }

        private void CleanUpLastRun()
        {
            var outputDirectory = _configuration["OutputDirectory"];
            var archiveDirectory = outputDirectory + "\\Archive";
            _fileUtilities.MoveDirectory(outputDirectory, archiveDirectory);
        }
    }
}
