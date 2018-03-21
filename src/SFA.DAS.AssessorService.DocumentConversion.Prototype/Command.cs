using System.Threading.Tasks;
using SFA.DAS.AssessorService.DocumentConversion.Prototype.Services;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype
{
    public class Command
    {
        private readonly CoverLetterTemplateService _createLetterHeads;
        private readonly IFACertificateService _printDataSpreadsheet;

        public Command(CoverLetterTemplateService createLetterHeads,
            IFACertificateService printDataSpreadsheet)
        {
            _createLetterHeads = createLetterHeads;
            _printDataSpreadsheet = printDataSpreadsheet;
        }

        public async Task Execute()
        {
            await _createLetterHeads.Create();
            _printDataSpreadsheet.Create();
        }
    }
}
