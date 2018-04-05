using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Services;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow
{
    public class Command
    {
        private readonly CoverLetterService _coverLetterService;
        private readonly IFACertificateService _ifaCertificateService;
        private readonly IConfiguration _configuration;

        public Command(CoverLetterService coverLetterService,
            IFACertificateService ifaCertificateService,
            IConfiguration configuration)
        {
            _coverLetterService = coverLetterService;
            _ifaCertificateService = ifaCertificateService;
            _configuration = configuration;
        }

        public async Task Execute()
        {
            await _coverLetterService.Create();
            await _ifaCertificateService.Create();
        }
    }
}
