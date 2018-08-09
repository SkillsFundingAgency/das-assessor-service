using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;

namespace SFA.DAS.AssessorService.Web.Staff.Tests
{
    public class WhenUserLoadsAmmendsGradePage : ContractAmmendQueryBase
    {     
        private IActionResult _result;

        [SetUp]
        public void Arrange()
        {                              
            _result = CertificateGradeController.Grade(Certificate.Id).GetAwaiter().GetResult();           
        }

        [Test]
        public void ThenShouldreturnValidViewModel()
        {
            var result = _result as ViewResult;
            var model = result.Model as CertificateGradeViewModel;

            var certificateData = JsonConvert.DeserializeObject<CertificateData>(Certificate.CertificateData);

            model.Id.Should().Be(Certificate.Id);
            model.SelectedGrade.Should().Be(certificateData.OverallGrade);            
        }
    }   
}
