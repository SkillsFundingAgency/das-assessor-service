﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Controllers.Private;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.UnitTests.PrivateCertificateTests.Queries
{
    public class Given_I_request_the_firstname_page : CertificateQueryBase
    {
        private IActionResult _result;
        private CertificateFirstNameViewModel _viewModelResponse;

        [SetUp]
        public void Arrange()
        {
            var certificatePrivateFirstNameController =
                new CertificatePrivateFirstNameController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    MockCertificateApiClient,
                    MockSession.Object
                    );

            SetupSession();

            _result = certificatePrivateFirstNameController.FirstName(false).GetAwaiter().GetResult();

            var result = _result as ViewResult;
            _viewModelResponse = result.Model as CertificateFirstNameViewModel;
        }

        [Test]
        public void ThenShouldReturnFirstName()
        {
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(Certificate.CertificateData);

            _viewModelResponse.Id.Should().Be(Certificate.Id);
            _viewModelResponse.FirstName.Should().Be(CertificateData.LearnerGivenNames);
        }
    }
}

