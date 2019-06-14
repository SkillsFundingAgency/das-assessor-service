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
    public class Given_I_submit_passed_certificate_with_no_receipient
    {
        private Certificate Certificate;
        private ViewResult _result;
        private CertificateCheckViewModelValidator _validator;

        [SetUp]
        public void Arrange()
        {
            Certificate = SetupCertificate();

            var certificateCheckController =
                new CertificateCheckController(Mock.Of<ILogger<CertificateController>>(),
                    SetupHttpContextAssessor(),
                    SetUpCertificateApiClient(),
                    SetupValidator(),
                    SetupSessionService()
                    );

            certificateCheckController.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var vm = SetupViewModel();
            _result = certificateCheckController.Check(vm).GetAwaiter().GetResult() as ViewResult;
        }

        private Certificate SetupCertificate()
        {
            var certificate = new Builder().CreateNew<Certificate>()
                .With(q => q.CertificateData = JsonConvert.SerializeObject(new Builder()
                    .CreateNew<CertificateData>()
                    .With(x => x.OverallGrade = CertificateGrade.Pass)
                    .With(x => x.AchievementDate = DateTime.Now)
                    .With(x => x.ContactName = null)
                    .With(x => x.ContactAddLine1 = null)
                    .With(x => x.ContactAddLine2 = null)
                    .With(x => x.ContactAddLine3 = null)
                    .With(x => x.ContactAddLine4 = null)
                    .With(x => x.ContactPostCode = null)
                    .Build()))
                .Build();

            var organisaionId = Guid.NewGuid();
            certificate.OrganisationId = organisaionId;

            var organisation = new Builder().CreateNew<Organisation>().Build();

            certificate.Organisation = organisation;

            return certificate;
        }

        private CertificateCheckViewModel SetupViewModel()
        {
            var viewModel = new CertificateCheckViewModel();
            viewModel.FromCertificate(Certificate);
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

        private CertificateCheckViewModelValidator SetupValidator()
        {
            var MockStringLocalizer = new MockStringLocaliserBuilder();

            var localiser = MockStringLocalizer.Build<CertificateCheckViewModelValidator>();

            return new CertificateCheckViewModelValidator(localiser.Object);
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

            MockSession.Setup(q => q.Get("CertificateSession")).Returns(serialisedCertificateSession);
            MockSession.Setup(q => q.Get("EndPointAsessorOrganisationId")).Returns("EPA00001");

            return MockSession.Object;
        }

        [Test]
        public void ThenShouldNotProgressToConfirmationPage()
        {
            _result.ViewName.Should().Be("~/Views/Certificate/Check.cshtml");
        }
    }
}
