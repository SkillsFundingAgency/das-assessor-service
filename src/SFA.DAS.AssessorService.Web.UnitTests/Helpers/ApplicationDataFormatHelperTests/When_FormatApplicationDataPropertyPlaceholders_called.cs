using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Helpers;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.UnitTests.Helpers.ApplicationDataFormatHelperTests
{
    [TestFixture]
    public class When_FormatApplicationDataPropertyPlaceholders_called
    {
        [Test]
        [TestCaseSource(nameof(PlaceholderTestSource))]
        public void Placeholders_are_replaced_with_applicaton_data(string textWithPlaceholders, string textWithReplacedPlaceholders, Dictionary<string, object> applicationData)
        {
            // Act
            var output = ApplicationDataFormatHelper.FormatApplicationDataPropertyPlaceholders(textWithPlaceholders, applicationData);

            // Assert
            output.Should().Be(textWithReplacedPlaceholders);
        }

        static IEnumerable<object[]> PlaceholderTestSource()
        {
            var first = 1;
            var second = "another";
            var third = 3;
            var fourth = DateTime.Now;

            var applicationData = new Dictionary<string, object>
            {
                { "First", first },
                { "Second", second },
                { "Third", third },
                { "Fourth", fourth },
            };

            var testSource = new[]
            {
                new object[] { "Text with {{First}} placeholder.", $"Text with {first} placeholder.", applicationData },
                new object[] { "Text with {{Second}} placeholder.", $"Text with {second} placeholder.", applicationData },
                new object[] { "Text with {{First}} placeholder and {{Second}} placeholder and placeholder {{Third}}.", $"Text with {first} placeholder and {second} placeholder and placeholder {third}.", applicationData },
                new object[] { "Text with {{Fourth:d MMM yyyy}} formatted date.", $"Text with {fourth:d MMM yyyy} formatted date.", applicationData },
                new object[] { "Text aligned '{{Third,4}}' aligned number.", $"Text aligned '{third,4}' aligned number.", applicationData },
                new object[] { "Text leading zero's {{First:00000}} placeholder.", $"Text leading zero's {first:00000} placeholder.", applicationData },

            };

            return testSource;
        }
    }
}
