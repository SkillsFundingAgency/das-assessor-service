using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Web.ViewModels.Apply;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.StandardControllerTests
{
    [TestFixture]
    public class When_Search_Is_Called : StandardControllerTestBase
    {
        [Test]
        public async Task Then_The_Results_Are_Returned()
        {
            // Arrange
            _mockStandardVersionApiClient
               .Setup(r => r.GetLatestStandardVersions())
               .ReturnsAsync(new List<StandardVersion> { 
                   new StandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.1"},
                   new StandardVersion { IFateReferenceNumber = "ST0002", Title = "Title 2", Version = "1.0"},
               });

            // Act
            var results = (await _sut.Search(new StandardVersionViewModel { StandardToFind = "Title" })) as ViewResult;

            // Assert
            var vm = results.Model as StandardVersionViewModel;
            Assert.AreEqual(2, vm.Results.Count);
            Assert.AreEqual("ST0001", vm.Results[0].IFateReferenceNumber);
            Assert.AreEqual("ST0002", vm.Results[1].IFateReferenceNumber);
        }
    }
}
