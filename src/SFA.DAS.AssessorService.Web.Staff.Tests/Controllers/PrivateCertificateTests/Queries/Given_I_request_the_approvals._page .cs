﻿using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Staff.Controllers;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Controllers.PrivateCertificateTests.Queries
{
    public class Given_I_request_the_approvals_page : CertificateQueryBase
    {
        private IActionResult _result;
        private CertificateApprovalViewModel _viewModelResponse;

        [OneTimeSetUp]
        public void Arrange()
        {
            MappingStartup.AddMappings();
            
            var   certificateApprovalsController =
                new CertificateApprovalsController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    MockApiClient                  
                    );
            
            _result = certificateApprovalsController.Approvals(new CertificatePostApprovalViewModel()).GetAwaiter().GetResult();

            var result = _result as ViewResult;
            _viewModelResponse = result.Model as CertificateApprovalViewModel;
        }

        [Test]
        public void ThenShouldReturnApprovals()
        {      
            _viewModelResponse.ApprovedCertificates.Count().Should().Be(3);            
        }

        [Test]
        public void ThenShouldReturnRejections()
        {
            _viewModelResponse.RejectedCertificates.Count().Should().Be(3);
        }

        [Test]
        public void ThenShouldReturnToBeApproveds()
        {
            _viewModelResponse.ToBeApprovedCertificates.Count().Should().Be(4);
        }
    }
}

