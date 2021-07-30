using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.UnitTests.Helpers;
using SFA.DAS.AssessorService.Web.UnitTests.MockedObjects;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using System;
using System.Collections.Generic;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Web.UnitTests.CertificateTests
{
    public class Given_I_post_the_date_of_a_failed_grade
    {
        private Certificate Certificate;
        private RedirectToActionResult _result;

        [SetUp]
        public void Arrange()
        {
            Certificate = SetupCertificate();

            var certificateDateController =
                new CertificateDateController(Mock.Of<ILogger<CertificateController>>(),
                    SetupHttpContextAssessor(),
                    SetUpCertificateApiClient(),
                    SetupValidator(),
                    SetupSessionService()
                    );

            var vm = SetupViewModel();

            _result = certificateDateController.Date(vm).GetAwaiter().GetResult() as RedirectToActionResult;
        }

        private Certificate SetupCertificate()
        {
            var certificate = new Builder().CreateNew<Certificate>()
                .With(q => q.CertificateData = JsonConvert.SerializeObject(new Builder()
                    .CreateNew<CertificateData>()
                    .With(x => x.OverallGrade = CertificateGrade.Fail)
                    .With(x => x.AchievementDate = null)
                    .Build()))
                .Build();

            var organisaionId = Guid.NewGuid();
            certificate.OrganisationId = organisaionId;

            var organisation = new Builder().CreateNew<Organisation>().Build();

            certificate.Organisation = organisation;

            return certificate;
        }

        private CertificateDateViewModel SetupViewModel()
        {
            var viewModel = new CertificateDateViewModel();
            viewModel.FromCertificate(Certificate);

            viewModel.Day = DateTime.Now.Day.ToString();
            viewModel.Month = DateTime.Now.Month.ToString();
            viewModel.Year = DateTime.Now.Year.ToString();

            return viewModel;
        }

        private IHttpContextAccessor SetupHttpContextAssessor()
        {
            return MockedHttpContextAccessor.Setup().Object;
        }

        private ICertificateApiClient SetUpCertificateApiClient()
        {
            return MockedCertificateApiClient.Setup(Certificate, new Mock<ILogger<CertificateApiClient>>());
        }

        private CertificateDateViewModelValidator SetupValidator()
        {
            var MockStringLocalizer = new MockStringLocaliserBuilder();

            var localiser = MockStringLocalizer.Build<CertificateDateViewModelValidator>();

            return new CertificateDateViewModelValidator(localiser.Object);
        }

        private ISessionService SetupSessionService()
        {
            var MockSession = new Mock<ISessionService>();

            var certificateSession = Builder<CertificateSession>
                .CreateNew()
                .With(q => q.CertificateId = Certificate.Id)
                .With(q => q.Options = new List<string>())
                .Build();

            var serialisedCertificateSession = JsonConvert.SerializeObject(certificateSession);

            MockSession.Setup(q => q.Get(nameof(CertificateSession))).Returns(serialisedCertificateSession);
            MockSession.Setup(q => q.Get("EndPointAsessorOrganisationId")).Returns("EPA00001");

            return MockSession.Object;
        }

        [Test]
        public void ThenShouldRedirectToCertificateCheckPage()
        {
            _result.ControllerName.Should().Be("CertificateCheck");
            _result.ActionName.Should().Be("Check");
        }
    }
}
