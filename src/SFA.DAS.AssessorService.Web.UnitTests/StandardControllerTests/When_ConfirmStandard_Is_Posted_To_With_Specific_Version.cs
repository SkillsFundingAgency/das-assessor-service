using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.ViewModels.Apply;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.StandardControllerTests
{
    [TestFixture]
    public class When_ConfirmStandard_Is_Posted_To_With_Specific_Version : StandardControllerTestBase
    {
        [Test]
        public async Task Then_UpdateStandardData_Is_Called()
        {
            // Arrange
            _mockOrgApiClient
             .Setup(r => r.GetAppliedStandardVersionsForEPAO(It.IsAny<string>(), "ST0001"))
             .ReturnsAsync(new List<AppliedStandardVersion> {
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.0M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.NotYetApplied},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.1M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.Approved},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.2M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.NotYetApplied},
             });

            // Act
            var model = new StandardVersionViewModel()
            {
                IsConfirmed = true,
            };
            await _sut.ConfirmStandard(model, "ST0001", "1.2");

            // Assert
            _mockApiClient.Verify(m => m.UpdateStandardData(It.IsAny<Guid>(), 1, "ST0001", "Title 1",
                It.Is<List<string>>(x => x.Count == 1 && x[0] == "1.2"), StandardApplicationTypes.Version));

            _mockQnaApiClient.Verify(m => m.UpdateApplicationData(It.IsAny<Guid>(), It.Is<ApplicationData>(x => x.ApplicationType == StandardApplicationTypes.Version)));
        }
    }
}
