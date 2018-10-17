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
    }   
}
