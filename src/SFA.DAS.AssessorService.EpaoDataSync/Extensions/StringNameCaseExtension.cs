using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SFA.DAS.AssessorService.EpaoDataSync.Extensions
{
    public static class StringNameCaseExtension
    {
        private static Dictionary<NameOptions, bool> Options = new Dictionary<NameOptions, bool>();
        private static readonly Dictionary<string, string> Exceptions = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> Replacements = new Dictionary<string, string>();
        private static readonly string[] Conjunctions = new string[] { "Y", "E", "I" };
        // Roman letters regexp.
        private static readonly string RomanRegex = @"\b((?:[Xx]{1,3}|[Xx][Ll]|[Ll][Xx]{0,3})?(?:[Ii]{1,3}|[Ii][VvXx]|[Vv][Ii]{0,3})?)\b";
        private static readonly Dictionary<char, char> AlternateCharacters = new Dictionary<char, char>() {
                {'’', '\''},
                {'‘','\'' },
                {'`', '\''},
                {'–', '-'},
                {'\u00A0',' '},
                {'\t',' '},
                {'%', ' '}
        };

        public enum NameOptions
        {
            lazy,
            spanish
        }

        static StringNameCaseExtension()
        {
            Options.Add(NameOptions.lazy, false);
            Options.Add(NameOptions.spanish, false);


            Exceptions.Add("\bMacEvicius", "Macevicius");
            Exceptions.Add("\bMacHado", "Machado");
            Exceptions.Add("\bMacHar", "Machar");
            Exceptions.Add("\bMacHin", "Machin");
            Exceptions.Add("\bMacHlin", "Machlin");
            Exceptions.Add("\bMacIas", "Macias");
            Exceptions.Add("\bMacIulis", "Maciulis");
            Exceptions.Add("\bMacKie", "Mackie");
            Exceptions.Add("\bMacKle", "Mackle");
            Exceptions.Add("\bMacKlin", "Macklin");
            Exceptions.Add("\bMacQuarie", "Macquarie");
            Exceptions.Add("\bMacOmber", "Macomber");
            Exceptions.Add("\bMacIn", "Macin");
            Exceptions.Add("\bMacKintosh", "Mackintosh");
            Exceptions.Add("\bMacKen", "Macken");
            Exceptions.Add("\bMacHen", "Machen");
            Exceptions.Add("\bMacHiel", "Machiel");
            Exceptions.Add("\bMacIol", "Maciol");
            Exceptions.Add("\bMacKell", "Mackell");
            Exceptions.Add("\bMacKlem", "Macklem");
            Exceptions.Add("\bMacKrell", "Mackrell");
            Exceptions.Add("\bMacLin", "Maclin");
            Exceptions.Add("\bMacKey", "Mackey");
            Exceptions.Add("\bMacKley", "Mackley");
            Exceptions.Add("\bMacHon", "Machon");

            Replacements.Add(@"\bAl(?=\s+\w)", "al");       // al Arabic or forename Al.
            Replacements.Add(@"\b(Bin|Binti|Binte)\b", "bin");      // bin, binti, binte Arabic
            Replacements.Add(@"\bAp\b", "ap");       // ap Welsh.
            //Replacements.Add(@"\bBen(?=\s+\w)", "ben");      // ben Hebrew or forename Ben.
            Replacements.Add(@"\bDell([ae])\b", "dell$1");   // della and delle Italian.
            Replacements.Add(@"\bD([aeiou])\b", "d$1");      // da, de, di Italian; du French; do Brasil
            Replacements.Add(@"\bD([ao]s)\b", "d$1");      // das, dos Brasileiros
            Replacements.Add(@"\bDe([lrn])\b", "de$1");     // del Italian; der/den Dutch/Flemish.
            Replacements.Add(@"\bEl\b", "el");       // el Greek or El Spanish.
            Replacements.Add(@"\bLa\b", "la");       // la French or La Spanish.
            Replacements.Add(@"\bL([eo])\b", "l$1");      // lo Italian; le French.
            Replacements.Add(@"\bVan(?=\s+\w)", "van");      // van German or forename Van.
            Replacements.Add(@"\bVon\b", "von");      // von Dutch/Flemish
            Replacements.Add(@"\bThe\b", "the");   //The to the
            Replacements.Add(@"\bOf\b", "of");    //Of to of
            Replacements.Add(@"\bAnd\b", "and");  //And to and
            Replacements.Add(@"\bSon\b", "son");  //Son to son
        }


        /// <summary>
        /// Main function for NameCase.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string NameCase(this string source, IDictionary<NameOptions, bool> options = null)
        {

            if (string.IsNullOrEmpty(source))
                return source;

            //Merge provided options with default options
            MergeOptions(options);

            Options.TryGetValue(NameOptions.lazy, out var lazy);
            CleanseStringForSpecialCharacters(ref source);
            // Do not do anything if string is mixed and lazy option is true.
            if (lazy && SkipMixed(ref source))
                return source;

            // Capitalize handles Irish names too
            Capitalize(ref source);

            // Fixes for "son (daughter) of" etc
            foreach (KeyValuePair<string, string> replacement in Replacements)
            {
                string pattern = replacement.Key;
                source = Regex.Replace(source, pattern, replacement.Value);
            }

            UpdateRoman(ref source);

            //For spanish names
            FixConjunction(ref source);

            return source;
        }

        /// <summary>
        /// Cleans all magic characters
        /// </summary>
        /// <returns></returns>
        public static void CleanseStringForSpecialCharacters(ref string inputString)
        {
            if (string.IsNullOrEmpty(inputString)) return;
                inputString.Trim();

            var specialCharacters = SpecialCharactersInString(inputString);

            if (specialCharacters.Length <= 0) return;

            foreach (var specialCharacter in specialCharacters)
            {
                var matchingEntry = AlternateCharacters.First(x => x.Key == specialCharacter);
                inputString = inputString.Replace(matchingEntry.Key, matchingEntry.Value);
                inputString.Trim();
            }
        }

        /// <summary>
        /// Find special characters in string
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        private static char[] SpecialCharactersInString(string inputString)
        {
            return AlternateCharacters.Where(kvp => inputString.Contains(kvp.Key)).Select(kvp => kvp.Key).ToArray();
        }

        /// <summary>
        /// Merge default options with provided optional options
        /// </summary>
        /// <param name="options"></param>
        private static void MergeOptions(IDictionary<NameOptions, bool> options)
        {
            if (options != null)
            {
                var tmpOptions = new Dictionary<NameOptions, bool>(Options);
                foreach (KeyValuePair<NameOptions, bool> kvp in options)
                {
                    tmpOptions[kvp.Key] = kvp.Value;
                }
                Options = tmpOptions;
            }
        }


        /// <summary>
        /// Skip if string is mixed case.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static bool SkipMixed(ref string source)
        {
            var firstLetterLower = source[0] == char.ToLower(source[0]);
            var allLowerOrUpper = ((source.ToLower()) == source || (source.ToUpper()) == source);
            return !(firstLetterLower || allLowerOrUpper);
        }

        /// <summary>
        /// Capitalize first letters.
        /// </summary>
        /// <param name="source"></param>
        private static void Capitalize(ref string source)
        {
            source = source.ToLower();

            source = Regex.Replace(source, @"(?:(M|m)(c)|(\b))([a-z])", delegate (Match m) {
                return String.Concat(m.Groups[1].Value.ToUpper(), m.Groups[2].Value, m.Groups[3].Value, m.Groups[4].Value.ToUpper());
            });

            UpdateMac(ref source);

            source.Replace("Macmurdo", "MacMurdo");
            source.Replace("MacIsaac", "MacIsaac");
        }


        /// <summary>
        /// Fix Spanish conjunctions.
        /// </summary>
        /// <param name="source"></param>
        private static void FixConjunction(ref string source)
        {
            Options.TryGetValue(NameOptions.spanish, out var spanish);
            if (!spanish)
                return;

            foreach (var conjunction in Conjunctions)
            {
                string pattern = @"\b" + conjunction + @"\b";

                source = Regex.Replace(source, pattern, conjunction.ToLower());
            }
        }

        /// <summary>
        /// Fix roman numeral names.
        /// </summary>
        /// <param name="source"></param>
        private static void UpdateRoman(ref string source)
        {
            var rgx = new Regex(RomanRegex);

            source = Regex.Replace(source, RomanRegex, delegate (Match m) {
                return m.Value.ToUpper();
            });
        }


        /// <summary>
        ///  Updates irish Mac & Mc.
        /// </summary>
        /// <param name="source"></param>
        private static void UpdateMac(ref string source)
        {
            foreach (KeyValuePair<string, string> exception in Exceptions)
            {
                var pattern = exception.Key;
                var rgx = new Regex(pattern);

                foreach (Match match in rgx.Matches(source))
                    source = source.Replace(match.Value, exception.Value);
            }
        }
    }
}
