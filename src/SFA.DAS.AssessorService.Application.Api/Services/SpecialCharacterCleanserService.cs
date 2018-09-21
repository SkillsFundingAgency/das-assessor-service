using System.Collections.Generic;
using System.Linq;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public class SpecialCharacterCleanserService : ISpecialCharacterCleanserService
    {
        private Dictionary<char, char> _alternateCharacters;

        public SpecialCharacterCleanserService()
        {
            BuildAlternates();
        }

        private void BuildAlternates()
        {
            _alternateCharacters = new Dictionary<char, char>
            {
                {'’', '\''},
                {'`', '\''},
                {'-', '–'},
                {'\u00A0',' '},
                {'\t',' '},
                {'%', ' '}
            };
        }

        public string CleanseStringForSpecialCharacters(string inputString)
        {
            var processedString = inputString;

            var specialCharacters = SpecialCharactersInString(processedString);

            if (specialCharacters.Length <= 0) return processedString;
            foreach (var specialCharacter in specialCharacters)
            {
                var matchingEntry = _alternateCharacters.First(x => x.Key == specialCharacter);
                processedString = processedString.Replace(matchingEntry.Key, matchingEntry.Value);
            }

            return processedString;
        }

        private char[] SpecialCharactersInString(string inputString)
        {
            return _alternateCharacters.Where(kvp => inputString.Contains(kvp.Key)).Select(kvp => kvp.Key).ToArray();
        }
    }
}
