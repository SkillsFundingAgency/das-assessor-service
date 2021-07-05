using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.ViewModels.Apply;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.StandardControllerTests
{
    [TestFixture]
    public class When_ConfirmStandard_Is_Called_With_Specific_Version : StandardControllerTestBase
    {
        [Test]
        public async Task Then_One_Version_Is_Returned()
        {
            // Arrange
            _mockOrgApiClient
              .Setup(r => r.GetAppliedStandardVersionsForEPAO(It.IsAny<string>(), "ST0001"))
              .ReturnsAsync(new List<AppliedStandardVersion> {
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.0M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.NotYetApplied},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.1M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.Approved},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.2M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.NotYetApplied},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.3M, LarsCode = 1, EPAChanged = true, ApprovedStatus = ApprovedStatus.NotYetApplied},
              });

            // Act
            var results = (await _sut.ConfirmStandard("ST0001", "1.2")) as ViewResult;

            // Assert
            var vm = results.Model as StandardVersionViewModel;
            Assert.AreEqual(1, vm.Results.Count);
            Assert.AreEqual("1.2", vm.Results[0].Version);
            Assert.AreEqual("1.2", vm.SelectedStandard.Version);

            Assert.AreEqual("~/Views/Application/Standard/ConfirmStandard.cshtml", results.ViewName);
        }
    }
}
