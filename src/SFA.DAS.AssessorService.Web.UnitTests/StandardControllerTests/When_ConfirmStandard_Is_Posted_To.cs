using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.ViewModels.Apply;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.StandardControllerTests
{
    [TestFixture]
    public class When_ConfirmStandard_Is_Posted_To : StandardControllerTestBase
    {
        [Test]
        public async Task Then_UpdateStandardData_Is_Called()
        {
            // Arrange
            _mockOrgApiClient
              .Setup(r => r.GetAppliedStandardVersionsForEPAO(It.IsAny<string>(), "ST0001"))
              .ReturnsAsync(new List<AppliedStandardVersion> {
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.0M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.NotYetApplied},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.1M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.NotYetApplied},
              });

            // Act
            var model = new StandardVersionViewModel()
            {
                IsConfirmed = true,
                SelectedVersions = new List<string>() { "1.1" }
            };
            await _sut.ConfirmStandard(model, Guid.NewGuid(), "ST0001", null);

            // Assert
            _mockApiClient.Verify(m => m.UpdateStandardData(It.IsAny<Guid>(), 1, "ST0001", "Title 1",
                It.Is<List<string>>(x => x.Count == 1 && x[0] == "1.1"), StandardApplicationTypes.Full));
        }

        [Test]
        public async Task Then_Error_If_Confirmed_Is_False()
        {
            // Arrange
            _mockOrgApiClient
              .Setup(r => r.GetAppliedStandardVersionsForEPAO(It.IsAny<string>(), "ST0001"))
              .ReturnsAsync(new List<AppliedStandardVersion> {
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.0M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.NotYetApplied},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.1M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.NotYetApplied},
              });

            // Act
            var model = new StandardVersionViewModel()
            {
                IsConfirmed = false,
                SelectedVersions = new List<string>() { "1.1" }
            };
            var result = (await _sut.ConfirmStandard(model, Guid.NewGuid(), "ST0001", null)) as ViewResult;

            // Assert
            Assert.AreEqual("~/Views/Application/Standard/ConfirmStandard.cshtml", result.ViewName);
            Assert.IsTrue(_sut.ModelState["IsConfirmed"].Errors.Any(x => x.ErrorMessage == "Confirm you have read the assessment plan"));
        }

        [Test]
        public async Task Then_Error_If_No_Versions_Selected()
        {
            // Arrange
            _mockOrgApiClient
              .Setup(r => r.GetAppliedStandardVersionsForEPAO(It.IsAny<string>(), "ST0001"))
              .ReturnsAsync(new List<AppliedStandardVersion> {
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.0M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.NotYetApplied},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.1M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.NotYetApplied},
              });

            // Act
            var model = new StandardVersionViewModel()
            {
                IsConfirmed = true,
                SelectedVersions = new List<string>() { }
            };
            var result = (await _sut.ConfirmStandard(model, Guid.NewGuid(), "ST0001", null)) as ViewResult;

            // Assert
            Assert.AreEqual("~/Views/Application/Standard/ConfirmStandard.cshtml", result.ViewName);
            Assert.IsTrue(_sut.ModelState["SelectedVersions"].Errors.Any(x => x.ErrorMessage == "You must select at least one version"));
        }
    }
}
