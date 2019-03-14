﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Staff.Controllers.Private;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Controllers.PrivateCertificateTests.Posts
{
    public class Given_I_post_the_ukprn_with_invalid_model : CertificatePostBase
    {
        private ViewResult _result;

        [SetUp]
        public void WhenInvalidModelContainsOneError()
        {
            var certificatePrivateProviderUkprnController =
                new CertificatePrivateProviderUkprnController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    MockApiClient                   
                );

            var vm = new CertificateUkprnViewModel
            {
                Id = Certificate.Id,
                Ukprn = "",
                IsPrivatelyFunded = true,
                ReasonForChange = "Required reason for change"
            };

            // view model validation errors will not be trigged as they are attached via fluent
            // validation and these are not connected in tests however this test is actually testing
            // the correct view is returned so the actual error is irrelevant and can be forced
            certificatePrivateProviderUkprnController.ModelState.AddModelError("", "Error");

            var result = certificatePrivateProviderUkprnController.Ukprn(vm).GetAwaiter().GetResult();

            _result = result as ViewResult;
        }

        [Test]
        public void ThenShouldReturnInvalidModelWithOneError()
        {
            _result.ViewData.ModelState.ErrorCount.Should().Be(1);
        }

        [Test]
        public void ThenShouldReturnUkprnView()
        {
            _result.ViewName.Should().Be("~/Views/CertificateAmend/Ukprn.cshtml");
        }
    }
}

