using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Staff.Controllers;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Controllers.PrivateCertificateTests.Queries
{
    public class Given_I_request_the_approvals_page : CertificateQueryBase
    {
        private IActionResult _result;
        private CertificateApprovalViewModel _viewModelResponse;
        private CertificateApprovalsController certificateApprovalsController;

        [OneTimeSetUp]
        public void Arrange()
        {
            MappingStartup.AddMappings();
            
            certificateApprovalsController =
                new CertificateApprovalsController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    MockApiClient                  
                    );
        }

        [Test]
        public void ShouldReturnApprovals()
        {
            var certificatePostApprovalViewModel = new CertificatePostApprovalViewModel
            {
                ActionHint = "Approved",
                ApprovalResults = new[]
                {
                    new ApprovalResult
                    {
                        CertificateReference = "SomeRef",
                        IsApproved = CertificateStatus.Submitted,
                        PrivatelyFundedStatus = CertificateStatus.Approved,
                        ReasonForChange = "SomeReason"
                    }
                }
            };

            _result = certificateApprovalsController.Approvals(certificatePostApprovalViewModel).GetAwaiter().GetResult();

            var result = _result as RedirectToActionResult;
            Assert.IsNotNull(result);
            result.ActionName.Should().Be("Approved");
            var returnResult = certificateApprovalsController.Approved(0).GetAwaiter().GetResult();
            var viewResult = returnResult as ViewResult;
            Assert.IsNotNull(viewResult);
            var viewModelResponse = viewResult.Model as CertificateApprovalViewModel;
            Assert.IsNotNull(viewModelResponse);
            viewModelResponse.ApprovedCertificates.Items.Count().Should().Be(3);            
        }

        [Test]
        public void ShouldReturnRejections()
        {
            var certificatePostApprovalViewModel = new CertificatePostApprovalViewModel
            {
                ActionHint = "Approved",
                ApprovalResults = new[]
                {
                    new ApprovalResult
                    {
                        CertificateReference = "SomeRef",
                        IsApproved = CertificateStatus.Draft,
                        PrivatelyFundedStatus = CertificateStatus.Rejected,
                        ReasonForChange = "SomeReason"
                    }
                }
            };

            _result = certificateApprovalsController.Approvals(certificatePostApprovalViewModel).GetAwaiter().GetResult();

            var result = _result as RedirectToActionResult;
            Assert.IsNotNull(result);
            result.ActionName.Should().Be("Rejected");
            var returnResult = certificateApprovalsController.Rejected(0).GetAwaiter().GetResult();
            var viewResult = returnResult as ViewResult;
            Assert.IsNotNull(viewResult);
            var viewModelResponse = viewResult.Model as CertificateApprovalViewModel;
            Assert.IsNotNull(viewModelResponse);

            viewModelResponse.RejectedCertificates.Items.Count.Should().Be(3);
        }

        [Test]
        public void ShouldReturnToBeApproveds()
        {
            var certificatePostApprovalViewModel = new CertificatePostApprovalViewModel
            {
                ActionHint = "Approved",
                ApprovalResults = new[]
                {
                    new ApprovalResult
                    {
                        CertificateReference = "SomeRef",
                        IsApproved = CertificateStatus.ToBeApproved,
                        PrivatelyFundedStatus = CertificateStatus.SentForApproval,
                        ReasonForChange = "SomeReason"
                    }
                }
            };

            _result = certificateApprovalsController.Approvals(certificatePostApprovalViewModel).GetAwaiter().GetResult();

            var result = _result as RedirectToActionResult;
            Assert.IsNotNull(result);
            result.ActionName.Should().Be("SentForApproval");
            var returnResult = certificateApprovalsController.SentForApproval(0).GetAwaiter().GetResult();
            var viewResult = returnResult as ViewResult;
            Assert.IsNotNull(viewResult);
            var viewModelResponse = viewResult.Model as CertificateApprovalViewModel;
            Assert.IsNotNull(viewModelResponse);
            viewModelResponse.SentForApprovalCertificates.Items.Count().Should().Be(4);
        }
    }
}

