using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public class CertificateNameCapitalisationService : ICertificateNameCapitalisationService
    {
        private readonly Dictionary<string, string> MacMcFamilyNameExceptions = new Dictionary<string, string>();
        private readonly Dictionary<string, string> NonEnglishFamilyNameReplacements = new Dictionary<string, string>();
        private readonly string[] Conjunctions = new string[] { "Y", "E", "I" };
        private readonly Dictionary<char, char> AlternateCharacters = new Dictionary<char, char>() {
                        {'’', '\''},
                        {'‘','\'' },
                        {'`', '\''},
                        {'–', '-'},
                        {'\u00A0',' '},
                        {'\t',' '},
                        {'%', ' '}
                };

        public CertificateNameCapitalisationService()
        {
            MacMcFamilyNameExceptions.Add(@"\bMacEvicius", "Macevicius");
            MacMcFamilyNameExceptions.Add(@"\bMacHado", "Machado");
            MacMcFamilyNameExceptions.Add(@"\bMacHar", "Machar");
            MacMcFamilyNameExceptions.Add(@"\bMacHin", "Machin");
            MacMcFamilyNameExceptions.Add(@"\bMacHlin", "Machlin");
            MacMcFamilyNameExceptions.Add(@"\bMacIas", "Macias");
            MacMcFamilyNameExceptions.Add(@"\bMacIulis", "Maciulis");
            MacMcFamilyNameExceptions.Add(@"\bMacKie", "Mackie");
            MacMcFamilyNameExceptions.Add(@"\bMacKle", "Mackle");
            MacMcFamilyNameExceptions.Add(@"\bMacKlin", "Macklin");
            MacMcFamilyNameExceptions.Add(@"\bMacQuarie", "Macquarie");
            MacMcFamilyNameExceptions.Add(@"\bMacOmber", "Macomber");
            MacMcFamilyNameExceptions.Add(@"\bMacIn", "Macin");
            MacMcFamilyNameExceptions.Add(@"\bMacKintosh", "Mackintosh");
            MacMcFamilyNameExceptions.Add(@"\bMacKen", "Macken");
            MacMcFamilyNameExceptions.Add(@"\bMacHen", "Machen");
            MacMcFamilyNameExceptions.Add(@"\bMacHiel", "Machiel");
            MacMcFamilyNameExceptions.Add(@"\bMacIol", "Maciol");
            MacMcFamilyNameExceptions.Add(@"\bMacKell", "Mackell");
            MacMcFamilyNameExceptions.Add(@"\bMacKlem", "Macklem");
            MacMcFamilyNameExceptions.Add(@"\bMacKrell", "Mackrell");
            MacMcFamilyNameExceptions.Add(@"\bMacLin", "Maclin");
            MacMcFamilyNameExceptions.Add(@"\bMacKey", "Mackey");
            MacMcFamilyNameExceptions.Add(@"\bMacKley", "Mackley");
            MacMcFamilyNameExceptions.Add(@"\bMacHon", "Machon");
            MacMcFamilyNameExceptions.Add(@"\bMacIejewska", "Maciejewska");
            MacMcFamilyNameExceptions.Add(@"\bMacHacek", "Machacek");
            MacMcFamilyNameExceptions.Add(@"\bMacAlova", "Macalova");
            MacMcFamilyNameExceptions.Add(@"\bMacEy", "Macey");
            MacMcFamilyNameExceptions.Add(@"\bMacIag", "Maciag");
            MacMcFamilyNameExceptions.Add(@"\bMacAnn", "Macann");
            MacMcFamilyNameExceptions.Add(@"\bMacHell", "Machell");
            MacMcFamilyNameExceptions.Add(@"\bMacLaren", "Maclaren");
            MacMcFamilyNameExceptions.Add(@"\bMacUgova", "Macugova");
            MacMcFamilyNameExceptions.Add(@"\bMacHajewski", "Machajewski");
            MacMcFamilyNameExceptions.Add(@"\bMacIazek", "Maciazek");
            MacMcFamilyNameExceptions.Add(@"\bMacHniak", "Machniak");
            MacMcFamilyNameExceptions.Add(@"\bMacEdo", "Macedo");

            NonEnglishFamilyNameReplacements.Add(@"\bAl(?=\s+\w)", "al");               // al Arabic or forename Al.
            NonEnglishFamilyNameReplacements.Add(@"\b(Bin|Binti|Binte)\b", "bin");      // bin, binti, binte Arabic
            NonEnglishFamilyNameReplacements.Add(@"\bAp\b", "ap");                      // ap Welsh.
            NonEnglishFamilyNameReplacements.Add(@"\bBen(?=\s+\w)", "ben");             // ben Hebrew or forename Ben.
            NonEnglishFamilyNameReplacements.Add(@"\bDell([ae])\b", "dell$1");          // della and delle Italian.
            NonEnglishFamilyNameReplacements.Add(@"\bD([aeiou])(?!-)\b", "d$1");        // da, de, di Italian; du French; do Brasil
            NonEnglishFamilyNameReplacements.Add(@"\bD([ao]s)\b", "d$1");               // das, dos Brasileiros
            NonEnglishFamilyNameReplacements.Add(@"\bDe([lrn])(?=\s+\w)", "de$1");      // del Italian; der/den Dutch/Flemish or forename Del.
            NonEnglishFamilyNameReplacements.Add(@"\bEl\b", "el");                      // el Greek or El Spanish.
            NonEnglishFamilyNameReplacements.Add(@"\bLa\b", "la");                      // la French or La Spanish.
            NonEnglishFamilyNameReplacements.Add(@"\bPar La Grâce\b", "par la grâce");  // par la grâce French.
            NonEnglishFamilyNameReplacements.Add(@"\bL([eo])\b", "l$1");                // lo Italian; le French.
            NonEnglishFamilyNameReplacements.Add(@"\bVan(?=\s+\w)", "van");             // van German or forename Van.
            NonEnglishFamilyNameReplacements.Add(@"\bVon\b", "von");                    // von Dutch/Flemish
            NonEnglishFamilyNameReplacements.Add(@"\bThe\b", "the");                    // The to the
            NonEnglishFamilyNameReplacements.Add(@"\bOf\b", "of");                      // Of to of
            NonEnglishFamilyNameReplacements.Add(@"\bAnd\b", "and");                    // And to and
            NonEnglishFamilyNameReplacements.Add(@"\bSon\b", "son");                    // Son to son
        }

        /// <summary>
        /// Replace characters in a name part with colloquial upper or lower case letters
        /// </summary>
        /// <param name="namePart">A part of name e.g. Given Name or Family Name</param>
        /// <param name="familyNamePart">Indicates whether special rules apply for a Family Name</param>
        /// <returns></returns>
        public string ProperCase(string namePart, bool familyNamePart = false)
        {
            if (string.IsNullOrEmpty(namePart))
                return namePart;

            CleanseStringForSpecialCharacters(ref namePart);
            Capitalize(ref namePart);

            if (familyNamePart)
            {
                UpdateMac(ref namePart);

                foreach (KeyValuePair<string, string> replacement in NonEnglishFamilyNameReplacements)
                {
                    string pattern = replacement.Key;
                    namePart = Regex.Replace(namePart, pattern, replacement.Value, RegexOptions.IgnoreCase);
                }

                FixConjunction(ref namePart);
            }

            return namePart;
        }

        /// <summary>
        /// Cleans all magic characters
        /// </summary>
        /// <returns></returns>
        private void CleanseStringForSpecialCharacters(ref string inputString)
        {
            if (string.IsNullOrEmpty(inputString)) return;
            inputString = inputString.Trim();

            var specialCharacters = SpecialCharactersInString(inputString);

            if (specialCharacters.Length <= 0) return;

            foreach (var specialCharacter in specialCharacters)
            {
                var matchingEntry = AlternateCharacters.First(x => x.Key == specialCharacter);
                inputString = inputString.Replace(matchingEntry.Key, matchingEntry.Value);
                inputString = inputString.Trim();
            }
        }

        /// <summary>
        /// Find special characters in string
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        private char[] SpecialCharactersInString(string inputString)
        {
            return AlternateCharacters.Where(kvp => inputString.Contains(kvp.Key)).Select(kvp => kvp.Key).ToArray();
        }

        /// <summary>
        /// Capitalize first letters.
        /// </summary>
        /// <param name="source"></param>
        private static void Capitalize(ref string source)
        {
            source = source.ToLower();

            source = Regex.Replace(source, @"(?:(^M|^m)(c)|(\b))([a-z])", delegate (Match m)
            {
                return String.Concat(m.Groups[1].Value.ToUpper(), m.Groups[2].Value, m.Groups[3].Value, m.Groups[4].Value.ToUpper());
            });
        }

        /// <summary>
        /// Fix Spanish conjunctions.
        /// </summary>
        /// <param name="source"></param>
        private void FixConjunction(ref string source)
        {
            foreach (var conjunction in Conjunctions)
            {
                string pattern = @"\b" + conjunction + @"\b";

                source = Regex.Replace(source, pattern, conjunction.ToLower());
            }
        }

        /// <summary>
        /// Updates irish/scottish Mac & Mc.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private void UpdateMac(ref string source)
        {
            // default to capital letter following 'c' as in "MacDonald"
            foreach (Match match in Regex.Matches(source, @"\b(Ma?c)([A-Za-z]{1})([A-Za-z]+)"))
            {
                if (match.Groups.Count == 4)
                {
                    source = Regex.Replace(source,
                        $@"\b{match.Groups[0]}",
                        $@"{match.Groups[1]}{match.Groups[2].ToString().ToUpper()}{match.Groups[3]}");
                }
            }

            // now fix "Mac" exceptions with a lower case letter following 'c' e.g. "Macevicius"
            foreach (var exception in MacMcFamilyNameExceptions.Keys)
            {
                source = Regex.Replace(source, exception, MacMcFamilyNameExceptions[exception]);
            }
        }
    }
}
