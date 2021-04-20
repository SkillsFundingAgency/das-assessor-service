using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Controllers;
using System;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Web.UnitTests.CertificateTests
{
    public class When_submitting_passed_certificate_with_no_option_selected : CertificateCheckControllerTestBase
    {
        private ViewResult _result;

        [SetUp]
        public void Arrange()
        {
            Certificate = SetupCertificate();

            _certificateCheckController = new CertificateCheckController(Mock.Of<ILogger<CertificateController>>(),
                    SetupHttpContextAssessor(),
                    SetUpCertificateApiClient(),
                    SetupValidator(),
                    SetupSessionService());

            _certificateCheckController.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            _viewModel = SetupViewModel();
        }

        private Certificate SetupCertificate()
        {
            var certificate = new Builder().CreateNew<Certificate>()
                .With(q => q.CertificateData = JsonConvert.SerializeObject(new Builder()
                    .CreateNew<CertificateData>()
                    .With(x => x.OverallGrade = CertificateGrade.Pass)
                    .With(x => x.AchievementDate = DateTime.Now)
                    .With(x => x.CourseOption = null)
                    .Build()))
                .Build();

            var organisaionId = Guid.NewGuid();
            certificate.OrganisationId = organisaionId;

            var organisation = new Builder().CreateNew<Organisation>().Build();

            certificate.Organisation = organisation;

            return certificate;
        }

        [Test]
        public void ThenShouldNotProgressToConfirmationPage()
        {
            _viewModel.Option = null;
            _viewModel.StandardHasOptions = true;

            _result = _certificateCheckController.Check(_viewModel).GetAwaiter().GetResult() as ViewResult;

            _result.ViewName.Should().Be("~/Views/Certificate/Check.cshtml");
        }
    }
}
