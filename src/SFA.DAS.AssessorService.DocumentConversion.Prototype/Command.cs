using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype
{
    public class Command
    {
        private readonly CreateLetterHeads _createLetterHeads;
        private readonly PrintDataSpreadsheet _printDataSpreadsheet;

        public Command(CreateLetterHeads createLetterHeads,
            PrintDataSpreadsheet printDataSpreadsheet)
        {
            _createLetterHeads = createLetterHeads;
            _printDataSpreadsheet = printDataSpreadsheet;
        }

        public async Task Execute()
        {
            await _createLetterHeads.Create();
            _printDataSpreadsheet.Execute();
        }
    }
}
