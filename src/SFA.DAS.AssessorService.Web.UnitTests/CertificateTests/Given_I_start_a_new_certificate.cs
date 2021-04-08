using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.UnitTests.MockedObjects;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.CertificateTests
{    
    public class Given_I_start_a_new_certificate
    {
        private Certificate _certificate;

        private CertificateController _controller;

        [SetUp]
        public void Arrange()
        {
            _certificate = SetupCertificate();

            _controller = new CertificateController(
                Mock.Of<ILogger<CertificateController>>(),
                SetupHttpContextAssessor(),
                SetUpCertificateApiClient(),
                // TO be replaced with setup and unit tests
                Mock.Of<IStandardVersionClient>(),
                Mock.Of<IStandardServiceClient>(),
                Mock.Of<ISessionService>());
        }

        [Test]
        public async Task Then_RedirectToChooseOptionPage()
        {
            var viewModel = new CertificateStartViewModel()
            {
                StdCode = _certificate.StandardCode,
                Uln = _certificate.Uln
            };

            var result = await _controller.Start(viewModel) as RedirectToActionResult;

            result.ActionName.Should().Be("Option");
            result.ControllerName.Should().Be("CertificateOption");
        }

        private IHttpContextAccessor SetupHttpContextAssessor()
        {
            return MockedHttpContextAccessor.Setup().Object;
        }

        private Certificate SetupCertificate()
        {
            var certificate = new Builder().CreateNew<Certificate>()
                .With(q => q.CertificateData = JsonConvert.SerializeObject(new Builder()
                    .CreateNew<CertificateData>()
                    .With(x => x.OverallGrade = null)
                    .With(x => x.AchievementDate = null)
                    .Build()))
                .Build();

            var organisaionId = Guid.NewGuid();
            certificate.OrganisationId = organisaionId;

            var organisation = new Builder().CreateNew<Organisation>().Build();

            certificate.Organisation = organisation;

            return certificate;
        }

        private ICertificateApiClient SetUpCertificateApiClient()
        {
            return MockedCertificateApiClient.Setup(_certificate, new Mock<ILogger<CertificateApiClient>>());
        }
    }
}
