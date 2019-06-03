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
    public class WhenUserLoadsAmendOptionPage : CertificateAmendQueryBase
    {     
        private IActionResult _result;

        [SetUp]
        public void Arrange()
        {
            var certificateGradeController = new CertificateOptionController(MockedLogger.Object, MockHttpContextAccessor.Object, ApiClient);
            _result = certificateGradeController.Option(Certificate.Id, true).GetAwaiter().GetResult();           
        }

        [Test]
        public void ThenShouldReturnValidViewModel()
        {
            var result = _result as ViewResult;
            var model = result.Model as CertificateOptionViewModel;

            var certificateData = JsonConvert.DeserializeObject<CertificateData>(Certificate.CertificateData);

            model.Id.Should().Be(Certificate.Id);
            model.Option.Should().Be(certificateData.CourseOption);            
        }
    }   
}
