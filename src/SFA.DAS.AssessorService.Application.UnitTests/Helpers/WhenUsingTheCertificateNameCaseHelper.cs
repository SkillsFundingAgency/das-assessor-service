using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Helpers;

namespace SFA.DAS.AssessorService.Application.UnitTests.Helpers
{
    public class WhenUsingTheCertificateNameCaseHelper
    {
        [TestCase(" Test")]
        [TestCase("     Test")]
        [TestCase("Test ")]
        [TestCase("Test     ")]
        [TestCase("  Test     ")]
        public void AndANameHasLeadingOrTrailingWhiteSpaces_ThenTheSpacesAreRemoved(string name)
        {
            var expectedName = name.Trim();

            var actualName = name.ProperCase(true);

            Assert.AreEqual(expectedName, actualName);
        }

        [TestCase("  ")]
        public void AndNameIsEmptyOrJustWhitespace_ThenEmptyStringReturned(string name)
        {
            var expectedName = "";

            var actualName = name.ProperCase(true);

            Assert.AreEqual(expectedName, actualName);
        }

        [TestCase("Smith\tJones", "Smith Jones")]
        [TestCase("O’Connor", "O\'Connor")]
        [TestCase("D‘Amato", "D\'Amato")]
        [TestCase("O`Reilly", "O\'Reilly")]
        [TestCase("Johnson-McCarthy", "Johnson-McCarthy")]
        [TestCase("Smith\u00A0Jones", "Smith Jones")]
        [TestCase("Smith%Jones", "Smith Jones")]
        public void AndNameHasSpecialCharacters_ThenTheSpecialCharactersAreReplacedCorrectly(string inputName, string expectedName)
        {
            var actualName = inputName.ProperCase(true);

            Assert.AreEqual(expectedName, actualName);
        }

        [TestCase("smith")]
        [TestCase("SMITH")]
        [TestCase("pienaar")]
        [TestCase("PIENAAR")]
        public void AndNameIsInAllLowercaseOrUppercase_ThenTheNameIsCapitalisedCorrectly(string name)
        {
            var expectedName = name.Substring(0, 1).ToUpper() + name.Substring(1, name.Length - 1).ToLower();

            var actualName = name.ProperCase(true);

            Assert.AreEqual(expectedName, actualName);
        }

        [TestCase("MacEvicius", "Macevicius")]
        [TestCase("MacHado", "Machado")]
        [TestCase("MacHar", "Machar")]
        [TestCase("MacHin", "Machin")]
        [TestCase("MacHlin", "Machlin")]
        [TestCase("Maclin", "Maclin")]
        [TestCase("MacIas", "Macias")]
        [TestCase("MacIulis", "Maciulis")]
        [TestCase("MacKie", "Mackie")]
        [TestCase("MacKle", "Mackle")]
        [TestCase("MacKlin", "Macklin")]
        [TestCase("MacQuarie", "Macquarie")]
        [TestCase("MacOmber", "Macomber")]
        [TestCase("MacIn", "Macin")]
        [TestCase("MacKintosh", "Mackintosh")]
        [TestCase("MacKen", "Macken")]
        [TestCase("MacHen", "Machen")]
        [TestCase("MacHiel", "Machiel")]
        [TestCase("MacIol", "Maciol")]
        [TestCase("MacKell", "Mackell")]
        [TestCase("MacKlem", "Macklem")]
        [TestCase("MacKrell", "Mackrell")]
        [TestCase("MacKey", "Mackey")]
        [TestCase("MacKley", "Mackley")]
        [TestCase("MacHon", "Machon")]
        [TestCase("MacIejewska", "Maciejewska")]
        [TestCase("MacHacek", "Machacek")]
        [TestCase("MacAlova", "Macalova")]
        [TestCase("MacEy", "Macey")]
        [TestCase("MacIag", "Maciag")]
        [TestCase("MacAnn", "Macann")]
        [TestCase("MacHell", "Machell")]
        [TestCase("MacLaren", "Maclaren")]
        [TestCase("MacUgova", "Macugova")]
        [TestCase("MacHajewski", "Machajewski")]
        [TestCase("MacIazek", "Maciazek")]
        [TestCase("MacHniak", "Machniak")]
        [TestCase("MacEdo", "Macedo")]
        public void AndNameHasMacMcPrefixAndIsOnTheExceptionsList_ThenTheNameIsCapitalisedCorrectly(string name, string expectedName)
        {
            var actualName = name.ProperCase(true);

            Assert.AreEqual(expectedName, actualName);
        }

        [TestCase("MacMillan", "MacMillan")]
        [TestCase("macmillan", "MacMillan")]
        [TestCase("MACMILLAN", "MacMillan")]
        [TestCase("McMullan", "McMullan")]
        [TestCase("mcmullan", "McMullan")]
        [TestCase("MCMULLAN", "McMullan")]
        public void AndNameHasMcOrMac_AndIsNotOnExceptionsList_ThenNameIsCapitalisedCorrectly(string name, string expectedName)
        {
            var actualName = name.ProperCase(true);

            Assert.AreEqual(expectedName, actualName);
        }

        [TestCase("Al-Malik", "Al-Malik")]
        [TestCase("Binti Abdullah", "bin Abdullah")]
        [TestCase("Ap Rhys", "ap Rhys")]
        [TestCase("Ben Avi", "ben Avi")]
        [TestCase("Della Rosa", "della Rosa")]
        [TestCase("De Luca", "de Luca")]
        [TestCase("Dos Santos", "dos Santos")]
        [TestCase("De La Cruz", "de la Cruz")]
        [TestCase("El Johnson", "el Johnson")]
        [TestCase("La Cruz", "la Cruz")]
        [TestCase("Par La Grâce Surname", "par la grâce Surname")]
        [TestCase("Lo Chen", "lo Chen")]
        [TestCase("Van Dyke", "van Dyke")]
        [TestCase("Von Dyke", "von Dyke")]
        [TestCase("The Family Name", "the Family Name")]
        [TestCase("Of Family Name", "of Family Name")]
        [TestCase("And Family Name", "and Family Name")]
        [TestCase("Son Family Name", "son Family Name")]
        public void AndNameIsNonEnglishAndIsOnTheRulesList_ThenTheNameIsCapitalisedCorrectly(string name, string expectedName)
        {
            var actualName = name.ProperCase(true);

            Assert.AreEqual(expectedName, actualName);
        }

        [TestCase("López y Martínez", "López y Martínez")]
        [TestCase("González e Iglesias", "González e Iglesias")]
        [TestCase("gonzález e iglesias", "González e Iglesias")]
        [TestCase("gonzález e IGLESIAS", "González e Iglesias")]
        public void AndNameHasSpanishConjuntions_ThenNameIsCapitalisedCorrectly(string name, string expectedName)
        {
            var actualName = name.ProperCase(true);

            Assert.AreEqual(expectedName, actualName);
        }
    }
}
