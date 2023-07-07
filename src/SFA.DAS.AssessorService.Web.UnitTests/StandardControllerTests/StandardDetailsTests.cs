using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Web.ViewModels.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.StandardControllerTests
{
    [TestFixture]
    public class StandardDetailsTests : StandardControllerTestBase
    {
        private List<StandardVersion> _allVersions;
        private List<StandardVersion> _approvedVersions;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            base.Arrange();

            _allVersions = new List<StandardVersion>()
            {
                new StandardVersion
                {
                    IFateReferenceNumber = "ST0001",
                    Title = "Standard Title One",
                    Version = "1.0",
                    VersionEarliestStartDate = DateTime.Now.AddDays(-10).Date,
                    VersionLatestEndDate = DateTime.Now.AddDays(10).Date
                },
                new StandardVersion
                {
                    IFateReferenceNumber = "ST0001",
                    Title = "Standard Title One",
                    Version = "1.1",
                    VersionEarliestStartDate = DateTime.Today.AddDays(-9),
                    VersionLatestEndDate = DateTime.Today.AddDays(11)
                }
            };

            _approvedVersions = new List<StandardVersion>()
            {
                new StandardVersion
                {
                    IFateReferenceNumber = "ST0001",
                    Title = "Standard Title One",
                    Version = "1.1",
                    VersionEarliestStartDate = DateTime.Today.AddDays(-9),
                    VersionLatestEndDate = DateTime.Today.AddDays(11)
                }
            };

            _mockStandardVersionApiClient.Setup(svc => svc.GetStandardVersionsByIFateReferenceNumber(It.IsAny<string>()))
                .ReturnsAsync((string referenceNumber) => _allVersions.Where(x => x.IFateReferenceNumber == referenceNumber).ToList());

            _mockStandardVersionApiClient.Setup(svc => svc.GetEpaoRegisteredStandardVersions(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string epaOrgId, string referenceNumber) => _approvedVersions.Where(x => x.IFateReferenceNumber == referenceNumber).ToList());
        }

        [Test]
        public async Task StandardDetails_ReturnsViewWithModel_WhenGetCalledWithValidReference()
        {
            // Act
            var result = await _sut.StandardDetails("ST0001");

            // Assert
            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = result as ViewResult;
            Assert.That(viewResult.Model, Is.TypeOf<StandardDetailsViewModel>());
            var model = viewResult.Model as StandardDetailsViewModel;

            Assert.AreEqual(_allVersions.FirstOrDefault(), model.SelectedStandard);
            Assert.AreEqual(_allVersions, model.AllVersions);
            Assert.AreEqual(_approvedVersions, model.ApprovedVersions);
        }

        [TestCase(null)]
        [TestCase("")]
        public void StandardDetails_ThrowsArgumentException_WhenGetCalledWithNullOrEmptyReference_(string referenceNumber)
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.StandardDetails(referenceNumber));
            Assert.That(ex.ParamName, Is.EqualTo("referenceNumber"));
        }

        [Test]
        public void StandardDetails_ReturnsBadRequest_WhenGetCalledWithNonExistentReference()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(async () => await _sut.StandardDetails("ST0003"));
        }
    }
}
