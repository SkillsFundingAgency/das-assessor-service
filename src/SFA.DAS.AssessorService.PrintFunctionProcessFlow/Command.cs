using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Services;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow
{
    public class Command
    {
        private readonly CoverLetterTemplateService _coverLetterTemplateService;
        private readonly IFACertificateService _ifaCertificateService;
        private readonly IConfiguration _configuration;

        public Command(CoverLetterTemplateService coverLetterTemplateService,
            IFACertificateService ifaCertificateService,
            IConfiguration configuration)
        {
            _coverLetterTemplateService = coverLetterTemplateService;
            _ifaCertificateService = ifaCertificateService;
            _configuration = configuration;
        }

        public async Task Execute()
        {
            await _coverLetterTemplateService.Create();
            await _ifaCertificateService.Create();
        }
    }
}
