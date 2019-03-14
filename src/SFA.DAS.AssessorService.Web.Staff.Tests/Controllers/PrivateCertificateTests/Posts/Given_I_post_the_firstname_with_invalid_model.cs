using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Staff.Controllers.Private;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Controllers.PrivateCertificateTests.Posts
{
    public class Given_I_post_the_firstname_with_invalid_model : CertificatePostBase
    {
        private ViewResult _result;

        [SetUp]
        public void WhenInvalidModelContainsOneError()
        {
            // view model validation are 
            var certificatePrivateFirstNameController =
                new CertificatePrivateFirstNameController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    MockApiClient                    
                );

            var vm = new CertificateFirstNameViewModel
            {
                Id = Certificate.Id,
                FullName = "James Corley",
                FirstName = "James", 
                FamilyName = "Corley",
                GivenNames = "James",
                Level = 2,
                Standard = "91",
                IsPrivatelyFunded = true,
                ReasonForChange = "Reason for change" 
            };

            // view model validation errors will not be trigged as they are attached via fluent
            // validation and these are not connected in tests however this test is actually testing
            // the correct view is returned so the actual error is irrelevant and can be forced
            certificatePrivateFirstNameController.ModelState.AddModelError("", "Error");
         
            var result = certificatePrivateFirstNameController.FirstName(vm).GetAwaiter().GetResult();
            _result = result as ViewResult;
        }

        [Test]
        public void ThenShouldReturnInvalidModelWithOneError()
        {
            _result.ViewData.ModelState.ErrorCount.Should().Be(1);
        }

        [Test]
        public void ThenShouldReturnFirstNameView()
        {
            _result.ViewName.Should().Be("~/Views/CertificateAmend/FirstName.cshtml");
        }        
    }
}

