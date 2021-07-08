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
    public class When_ConfirmStandard_Is_Called_With_Approved_Versions : StandardControllerTestBase
    {
        [Test]
        public async Task Then_All_Versions_For_Standard_Are_Returned()
        {
            // Arrange
            _mockOrgApiClient
               .Setup(r => r.GetAppliedStandardVersionsForEPAO(It.IsAny<string>(), "ST0001"))
               .ReturnsAsync(new List<AppliedStandardVersion> { 
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.0M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.NotYetApplied},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.1M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.Approved},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.2M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.NotYetApplied},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.3M, LarsCode = 1, EPAChanged = true, ApprovedStatus = ApprovedStatus.ApplyInProgress},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.4M, LarsCode = 1, EPAChanged = true, ApprovedStatus = ApprovedStatus.NotYetApplied},
               });

            // Act
            var results = (await _sut.ConfirmStandard("ST0001", null)) as ViewResult;

            // Assert
            var vm = results.Model as StandardVersionApplicationViewModel;
            Assert.AreEqual(5, vm.Results.Count);
            Assert.AreEqual("1.4", vm.Results[0].Version);
            Assert.AreEqual(VersionStatus.NewVersionChanged, vm.Results[0].VersionStatus);
            Assert.AreEqual("1.3", vm.Results[1].Version);
            Assert.AreEqual(VersionStatus.InProgress, vm.Results[1].VersionStatus);
            Assert.AreEqual("1.2", vm.Results[2].Version);
            Assert.AreEqual(VersionStatus.NewVersionNoChange, vm.Results[2].VersionStatus);
            Assert.AreEqual("1.1", vm.Results[3].Version);
            Assert.AreEqual(VersionStatus.Approved, vm.Results[3].VersionStatus);
            Assert.AreEqual("1.0", vm.Results[4].Version);
            Assert.Null(vm.Results[4].VersionStatus);

            Assert.AreEqual("~/Views/Application/Standard/StandardVersion.cshtml", results.ViewName);
        }
    }
}
