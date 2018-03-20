using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype
{
    public class Command
    {
        private readonly CreateLetterHeads _createLetterHeads;

        public Command(CreateLetterHeads createLetterHeads)
        {
            _createLetterHeads = createLetterHeads;
        }

        public async Task Execute()
        {
            await _createLetterHeads.Create();
        }
    }
}
