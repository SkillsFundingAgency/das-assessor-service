using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;
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
                {'‘','\'' },
                {'`', '\''},
                {'-', '–'},
                {'\u00A0',' '},
                {'\t',' '},
                {'%', ' '}
            };
        }

        public string CleanseStringForSpecialCharacters(string inputString)
        {
            if (string.IsNullOrEmpty(inputString)) return null;
            var processedString = inputString.Trim();

            var specialCharacters = SpecialCharactersInString(processedString);

            if (specialCharacters.Length <= 0) return processedString;

            foreach (var specialCharacter in specialCharacters)
            {
                var matchingEntry = _alternateCharacters.First(x => x.Key == specialCharacter);
                processedString = processedString.Replace(matchingEntry.Key, matchingEntry.Value);
            }

            return processedString.Trim();
        }

        public string UnescapeAndRemoveNonAlphanumericCharacters(string text)
        {

            var unescapedText = "";
            if (!string.IsNullOrEmpty(text))
                unescapedText = Uri.UnescapeDataString(text?.Trim());
            return string.Concat(unescapedText.Where(char.IsLetterOrDigit));
        }

        private char[] SpecialCharactersInString(string inputString)
        {
            return _alternateCharacters.Where(kvp => inputString.Contains(kvp.Key)).Select(kvp => kvp.Key).ToArray();
        }
    }
}
