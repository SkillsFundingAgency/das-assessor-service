using System;
using NUnit.Framework;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using SFA.DAS.AssessorService.Web.Staff.Helpers;


namespace SFA.DAS.AssessorService.Web.Staff.Tests.Helpers
{
    [TestFixture]
    public class RegisterValidationCheckDateEntryValidTests
    {
        private readonly RegisterValidator _validator = new RegisterValidator(); 

        [TestCase("1","1","2018", true)]
        [TestCase("","","", true)]
        [TestCase("2","","", false)]
        [TestCase("","3","", false)]
        [TestCase("","","20", false)]
        [TestCase("2","3","", false)]
        [TestCase("2","","10", false)]        
        [TestCase("2","","2018", false)]
        [TestCase("50","12","2018", false)]
        [TestCase("2","50","2018", false)]
        [TestCase("2","2","1018", false)]
        [TestCase("29","2","2018", false)]
        [TestCase("28","2","2018", true)]
        [TestCase("1","3","2018", true)]
        public void RegisterValidationOfDateEntriesReturnIssueWhenCalled(string day, string month, string year, bool IsValid)
        {
            var result = _validator.CheckDateIsEmptyOrValid(day, month, year, "dayFieldName", "monthFieldName", "yearFieldName",
                "dateFieldName", "Field Description");
            
            Assert.AreEqual(IsValid,result.IsValid);
        }
    }
}