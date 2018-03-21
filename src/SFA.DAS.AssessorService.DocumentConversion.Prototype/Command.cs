using System.Threading.Tasks;
using SFA.DAS.AssessorService.DocumentConversion.Prototype.Services;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype
{
    public class Command
    {
        private readonly CoverLetterTemplateService _coverLetterTemplateService;
        private readonly IFACertificateService _ifaCertificateService;

        public Command(CoverLetterTemplateService coverLetterTemplateService,
            IFACertificateService ifaCertificateService)
        {
            _coverLetterTemplateService = coverLetterTemplateService;
            _ifaCertificateService = ifaCertificateService;
        }

        public async Task Execute()
        {
            await _coverLetterTemplateService.Create();
            _ifaCertificateService.Create();
        }
    }
}
