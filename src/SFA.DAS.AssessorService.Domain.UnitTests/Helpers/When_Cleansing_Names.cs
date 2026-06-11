using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Helpers;
using FluentAssertions;

namespace SFA.DAS.AssessorService.Domain.UnitTests.Helpers
{
    public class When_Cleansing_Names
    {
        [TestCase("John  Smith", "John Smith")]
        [TestCase("Mary–Jane", "Mary-Jane")]
        [TestCase("O?Connor", "O'Connor")]
        [TestCase("O`Brien", "O'Brien")]
        [TestCase("John~|Smith", "JohnSmith")]
        [TestCase("Anna_Brown", "Anna-Brown")]
        [TestCase("Tom = Jones", "Tom-Jones")]
        [TestCase("Peter\\Brown", "PeterBrown")]
        [TestCase("Smith,John", "Smith,John")]
        [TestCase("  David   Clark  ", "David Clark")]
        [TestCase("Mark - Spencer", "Mark-Spencer")]
        [TestCase("Mark- Spencer", "Mark-Spencer")]
        [TestCase("A\u200BB", "AB")]
        [TestCase("A\u00A0B", "A B")]
        [TestCase("A\u0009B", "A B")]
        [TestCase("  O?Brien\u00A0  - Smith  ", "O'Brien-Smith")]
        [TestCase("John‘Doe`'", "John'Doe")]
        [TestCase("A#B__C\\D|||E~~F", "AB--CDEF")]
        [TestCase("  Alice   Bob  ", "Alice Bob")]
        public void CleanseName_returns_expected_result(string input, string expected)
        {
            NameCleaner.CleanseName(input).Should().Be(expected);
        }

        [Test]
        public void Returns_null_when_input_is_null()
        {
            NameCleaner.CleanseName(null).Should().BeNull();
        }
    }
}