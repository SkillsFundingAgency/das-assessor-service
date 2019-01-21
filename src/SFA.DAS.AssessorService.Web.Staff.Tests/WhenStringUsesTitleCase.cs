using System;
using System.Globalization;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Staff.Controllers;
using SFA.DAS.AssessorService.Web.Staff.Tests.Controllers;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;

namespace SFA.DAS.AssessorService.Web.Staff.Tests
{
    [TestFixture]
    public class WhenStringUsesTitleCase
    {           
        [Test]
        [TestCase(@"aBRAM aRAMOT")]   
        [TestCase(@"AbRAM ARAMOT")]        
        [TestCase(@"ABRAM ArAMOT")]        
        [TestCase(@"ABRAm ARAMot")]        
        [TestCase(@"abram aramot")]        
        [TestCase(@"ABRAM ARamot")]        
        public void ThenShouldReturnValidViewModel(string testString)
        {
            var result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(testString.ToLower());
            
            Assert.AreEqual(result, "Abram Aramot");
            Console.WriteLine(result);
        }


        [TestCase("JAMES SMITH", "James Smith")]
        [TestCase("JAMES O'SMITH", "James O'Smith")]
        [TestCase("James O'Smith", "James O'Smith")]
        [TestCase("James o'Smith", "James o'Smith")]
        [TestCase("Oscar de la Hoya", "Oscar de la Hoya")]
        [TestCase("abram aramot", "Abram Aramot")]
        [TestCase("Abram ARAMOT", "Abram ARAMOT")]
        public void ShouldReturnTitleCaseIfNotAllUpper(string input, string expected)
        {
            var result = NameToTitleCase(input);
            result.Should().Be(expected);
        }
        
        
        
        private static string NameToTitleCase(string name)
        {
            var isAllLower = true;
            var isAllUpper = true;
            
            foreach (var character in name)
            {
                if (char.IsLetter(character) &&  !char.IsUpper(character))
                {
                    isAllUpper = false;
                }
            }
            
            foreach (var character in name)
            {
                if (char.IsLetter(character) &&  
                    char.IsUpper(character))
                {
                    isAllLower = false;
                }
            }

            if (!isAllUpper && !isAllLower) return name;
            var chars = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower()).ToCharArray();

            for (var i = 0; i + 1 < chars.Length; i++)
            {
                if (chars[i].Equals('\'') ||
                    chars[i].Equals('`') ||
                    chars[i].Equals('-'))
                {                    
                    chars[i + 1] = char.ToUpper(chars[i + 1]);
                }
            }
            return new string(chars);
        }    
        
    }   
    
    
}
