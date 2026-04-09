using System.Linq;

namespace SFA.DAS.AssessorService.Domain.Helpers
{
    public static class NameCleaner
    {
        // NOTE: This cleansing logic mirrors database cleansing rules.
        // Keep this method in sync with CleanseName.sql and update callers/tests
        // whenever the rules here are changed.
        public static string CleanseName(string name)
        {
            if (name == null) return null;

            var translated = new string(name.Select(c => c switch
            {
                '‘' => '\'',
                '`' => '\'',
                '’' => '\'',
                '–' => '-',
                '_' => '-',
                '=' => '-',
                '\\' => '#',
                '|' => '#',
                '~' => '#',
                '\u200B' => '#',
                '\u00A0' => ' ',
                '\u0009' => ' ',
                _ => c
            }).ToArray());

            var result = translated;

            result = result.Replace("#", "");
            result = result.Replace("  ", " ");
            result = result.Replace("O?", "O'");
            result = result.Trim();
            result = result.Replace("'-", "");
            result = result.Replace("  ", " ");
            result = result.Replace(" -", "-");
            result = result.Replace("- ", "-");
            result = result.Trim();
            result = result.TrimEnd('\'');

            return result;
        }
    }
}