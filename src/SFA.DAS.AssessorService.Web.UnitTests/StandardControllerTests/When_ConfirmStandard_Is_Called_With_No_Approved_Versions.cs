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
    public class When_ConfirmStandard_Is_Called_With_No_Approved_Versions : StandardControllerTestBase
    {
        [Test]
        public async Task Then_All_Versions_For_Standard_Are_Returned()
        {
            // Arrange
            _mockOrgApiClient
               .Setup(r => r.GetAppliedStandardVersionsForEPAO(It.IsAny<string>(), "ST0001"))
               .ReturnsAsync(new List<AppliedStandardVersion> {
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.0M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.NotYetApplied},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.1M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.NotYetApplied},
               });

            // Act
            var results = (await _sut.ConfirmStandard(Guid.NewGuid(), "ST0001", null)) as ViewResult;

            // Assert
            var vm = results.Model as StandardVersionViewModel;
            Assert.AreEqual(2, vm.Results.Count);
            Assert.AreEqual("1.1", vm.SelectedStandard.Version);
            Assert.AreEqual("~/Views/Application/Standard/ConfirmStandard.cshtml", results.ViewName);
        }
    }
}
