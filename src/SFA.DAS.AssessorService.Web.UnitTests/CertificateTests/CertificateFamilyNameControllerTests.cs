﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using SFA.DAS.Testing.AutoFixture;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using SFA.DAS.AssessorService.Domain.Entities;
using FluentAssertions;
using SFA.DAS.AssessorService.Domain.JsonData;
using FluentAssertions.Execution;
using SFA.DAS.AssessorService.Web.Validators;

namespace SFA.DAS.AssessorService.Web.UnitTests.CertificateTests
{
    [TestFixture]
    public class CertificateFamilyNameControllerTests
    {
        private CertificateFamilyNameController _controller;
        private Mock<IHttpContextAccessor> _mockContextAccessor;
        private Mock<ISessionService> _mockSessionService;
        private Mock<ICertificateApiClient> _mockCertificateApiClient;
        private Mock<CertificateFamilyNameViewModelValidator> _validator;

        private const int Ukprn = 123456;
        private const string Username = "TestProviderUsername";
        private readonly Guid CertificateId = Guid.NewGuid();
        private const string EpaoId = "EPAO123";


        [SetUp]
        public void Setup()
        {
            _mockContextAccessor = new Mock<IHttpContextAccessor>();
            _mockSessionService = new Mock<ISessionService>();
            _mockCertificateApiClient = new Mock<ICertificateApiClient>();
            _validator = new Mock<CertificateFamilyNameViewModelValidator>(_mockCertificateApiClient.Object);

            _mockContextAccessor.Setup(s => s.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")).Returns(new Claim("", Ukprn.ToString()));
            _mockContextAccessor.Setup(s => s.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")).Returns(new Claim("", Username));
            _mockContextAccessor.Setup(s => s.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")).Returns(new Claim("", EpaoId));
            _mockContextAccessor.Setup(s => s.HttpContext.Request.Query).Returns(Mock.Of<IQueryCollection>());
            _mockCertificateApiClient.Setup(s => s.GetCertificate(It.IsAny<Guid>(), false)).ReturnsAsync(
                new Certificate
                {
                    Id = CertificateId,
                    CertificateData = new CertificateData() { LearnerFamilyName = "FamilyName" }
                });

            _controller = new CertificateFamilyNameController(Mock.Of<ILogger<CertificateController>>(), _mockContextAccessor.Object, _mockCertificateApiClient.Object, _mockSessionService.Object, _validator.Object);
        }

        [Test, MoqAutoData]
        public async Task WhenRequestingFamilyNameView_ThenLoadsFamilyNameViewSuccessfully(CertificateSession session)
        {
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            var result = await _controller.FamilyName() as ViewResult;

            using (new AssertionScope("The view is loaded successfully"))
            {
                result.ViewName.Should().Be("~/Views/Certificate/FamilyName.cshtml");
                result.Model.Should().BeOfType<CertificateFamilyNameViewModel>();
            }

        }

        [Test]
        public async Task WhenRequestingFamilyNameView_AndThereIsNoSessionFound_ThenRedirectsToSearch()
        {
            var result = await _controller.FamilyName() as RedirectToActionResult;

            using (new AssertionScope("User is redirected to search"))
            {
                result.ControllerName.Should().Be("Search");
                result.ActionName.Should().Be("Index");
            }
        }


        [Test, MoqAutoData]
        public async Task WhenPostingFamilyNameView_AndInputIsValid_ThenRedirectsToCheckPage(CertificateFamilyNameViewModel mockViewModel)
        {
            mockViewModel = PrepareValidViewModel(mockViewModel);

            var result = await _controller.FamilyName(mockViewModel) as RedirectToActionResult;

            using (new AssertionScope("User is redirected to check page"))
            {
                result.ControllerName.Should().Be("CertificateCheck");
                result.ActionName.Should().Be("Check");
            }

        }

        [Test]
        [MoqInlineAutoData("")]
        [MoqInlineAutoData("FamilyNames")]
        public async Task WhenPostingFamilyNameView_AndInputIsNotValid_ThenRefreshesWithErrors(string inputFamilyName, CertificateFamilyNameViewModel mockViewModel)
        {
            mockViewModel.FamilyName = inputFamilyName;

            var result = await _controller.FamilyName(mockViewModel) as ViewResult;

            using (new AssertionScope("Page refreshes with validation errors"))
            {
                result.ViewName.Should().Be("/Views/Certificate/FamilyName.cshtml");
                result.ViewData.ModelState.ErrorCount.Should().BeGreaterThanOrEqualTo(1);
            }

        }

        private CertificateFamilyNameViewModel PrepareValidViewModel(CertificateFamilyNameViewModel viewModel)
        {
            var certData = _mockCertificateApiClient.Object.GetCertificate(viewModel.Id).Result.CertificateData;
            viewModel.FamilyName = certData.LearnerFamilyName;
            return viewModel;
        }

    }
}
