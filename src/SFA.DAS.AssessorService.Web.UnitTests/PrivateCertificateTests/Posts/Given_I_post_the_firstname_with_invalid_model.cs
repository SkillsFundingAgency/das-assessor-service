﻿using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Controllers.Private;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.UnitTests.PrivateCertificateTests.Posts
{
    public class Given_I_post_the_firstname_with_invalid_model : CertificatePostBase
    {
        private ViewResult _result;

        [SetUp]
        public void Arrange()
        {
            var certificatePrivateFirstNameController =
                new CertificatePrivateFirstNameController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    MockCertificateApiClient,
                    MockSession.Object
                );

            var vm = new CertificateFirstNameViewModel
            {
                Id = Certificate.Id,
                FullName = "James Corley",
                FirstName = String.Empty,
                FamilyName = "",
                GivenNames = "James",
                Level = 2,
                Standard = "91",
                IsPrivatelyFunded = true
            };

            SetupSession();
         
            certificatePrivateFirstNameController.ModelState.AddModelError("", "Error");

            var result = certificatePrivateFirstNameController.FirstName(vm).GetAwaiter().GetResult();

            _result = result as ViewResult;
        }

        [Test]
        public void ThenShouldReturnInvalidModelWithOneError()
        {
            _result.ViewData.ModelState.ErrorCount.Should().Be(1);
        }        
    }
}

