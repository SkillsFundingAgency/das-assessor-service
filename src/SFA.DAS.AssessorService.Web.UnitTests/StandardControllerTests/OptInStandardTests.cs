using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using SFA.DAS.AssessorService.Web.ViewModels.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.StandardControllerTests
{
     [TestFixture]
    public class OptInStandardTests : StandardControllerTestBase
    {
        private List<StandardVersion> _approvedVersions;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            base.Arrange();

            var standardVersions = new List<StandardVersion>()
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

            _mockStandardVersionApiClient.Setup(svc => svc.GetEpaoRegisteredStandardVersions(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string epaOrgId, string referenceNumber) => _approvedVersions.Where(x => x.IFateReferenceNumber == referenceNumber).ToList());

            _mockStandardVersionApiClient.Setup(svc => svc.GetStandardVersionsByIFateReferenceNumber(It.IsAny<string>()))
                .ReturnsAsync((string referenceNumber) => standardVersions.Where(x => x.IFateReferenceNumber == referenceNumber).ToList());
        }

        [Test]
        public async Task OptInStandardVersion_ReturnsViewWithModel_WhenGetCalledWithValidReferenceAndVersion([ValueSource(nameof(TestDataForOptInStandardVersion))] TestDataOptInStandardVersion testData)
        {
            // Act
            var result = await _sut.OptInStandardVersion(testData.ReferenceNumber, testData.Version);

            // Assert
            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = result as ViewResult;
            Assert.That(viewResult.Model, Is.TypeOf<OptInStandardVersionViewModel>());
            var model = viewResult.Model as OptInStandardVersionViewModel;

            // Assert all fields are populated correctly
            Assert.AreEqual(testData.ReferenceNumber, model.StandardReference);
            Assert.AreEqual(testData.ExpectedTitle, model.StandardTitle);
            Assert.AreEqual(testData.ExpectedVersion, model.Version);
            Assert.AreEqual(testData.ExpectedVersionEarliestStartDate, model.EffectiveFrom);
            Assert.AreEqual(testData.ExpectedVersionLatestEndDate, model.EffectiveTo);
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task OptInStandardVersion_ReturnsBadRequest_WhenGetCalledWithNullOrEmptyReference(string referenceNumber)
        {
            var result = await _sut.OptInStandardVersion(referenceNumber, "1.0");

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task OptInStandardVersion_ReturnsBadRequest_WhenGetCalledWithNullOrEmptyVersion(string version)
        {
            var result = await _sut.OptInStandardVersion("ST0001", version);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task OptInStandardVersion_ReturnsBadRequest_WhenGetCalledWithNonExistentVersion()
        {
            var result = await _sut.OptInStandardVersion("ST0001", "2.0");

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task OptInStandardVersion_ReturnsRedirectResult_WhenPostCalledWithValidModel()
        {
            // Arrange
            var model = new OptInStandardVersionViewModel
            {
                StandardReference = "ST0001",
                Version = "1.0",
                EffectiveFrom = DateTime.Today.AddDays(-5),
                EffectiveTo = DateTime.Today.AddDays(5)
            };

            var result = await _sut.OptInStandardVersion(model);

            Assert.That(result, Is.TypeOf<RedirectToRouteResult>());
            
            var redirectResult = result as RedirectToRouteResult;
            Assert.That(redirectResult.RouteName, Is.EqualTo(StandardController.OptInStandardVersionConfirmationRouteGet));
            Assert.That(redirectResult.RouteValues["referenceNumber"], Is.EqualTo(model.StandardReference));
            Assert.That(redirectResult.RouteValues["version"], Is.EqualTo(model.Version));
        }

        [Test]
        public async Task OptInStandardVersion_ReturnsBadRequest_WhenPostCalledWithNullModel()
        {
            var result = await _sut.OptInStandardVersion(null);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task OptInStandardVersion_ReturnsBadRequest_WhenPostCalledWithNullOrEmptyStandardReference(string standardReference)
        {
            var model = new OptInStandardVersionViewModel
            {
                StandardReference = standardReference,
                Version = "1.0"
            };

            var result = await _sut.OptInStandardVersion(model);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task OptInStandardVersion_ReturnsBadRequest_WhenPostCalledWithNullOrEmptyVersion(string version)
        {
            var model = new OptInStandardVersionViewModel
            {
                StandardReference = "ST0001",
                Version = version
            };

            var result = await _sut.OptInStandardVersion(model);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task OptInStandardVersion_ReturnsBadRequest_WhenPostCalledWithExistingVersionForStandard()
        {
            var model = new OptInStandardVersionViewModel
            {
                StandardReference = "ST0001",
                Version = "1.1"
            };

            var result = await _sut.OptInStandardVersion(model);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task OptInStandardVersionConfirmation_ReturnsViewWithModel_WhenGetCalledWithValidReferenceAndVersion()
        {
            // Act
            var result = await _sut.OptInStandardVersionConfirmation("ST0001", "1.0");

            // Assert
            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = result as ViewResult;
            Assert.That(viewResult.Model, Is.TypeOf<OptInStandardVersionConfirmationViewModel>());
            var model = viewResult.Model as OptInStandardVersionConfirmationViewModel;

            Assert.AreEqual("Standard Title One", model.StandardTitle);
            Assert.AreEqual("ST0001", model.StandardReference);
            Assert.AreEqual("1.0", model.Version);
            Assert.AreEqual("http://feedback-url.com", model.FeedbackUrl);
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task OptInStandardVersionConfirmation_ReturnsBadRequest_WhenGetCalledWithNullOrEmptyReference(string referenceNumber)
        {
            var result = await _sut.OptInStandardVersionConfirmation(referenceNumber, "1.0");

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task OptInStandardVersionConfirmation_ReturnsBadRequest_WhenGetCalledWithNullOrEmptyVersion(string version)
        {
            var result = await _sut.OptInStandardVersionConfirmation("Ref123", version);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task OptInStandardVersionConfirmation_ReturnsBadRequest_WhenGetCalledWithNonExistentReference()
        {
            var result = await _sut.OptInStandardVersionConfirmation("RefNonExistent", "1.0");

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }


        public class TestDataOptInStandardVersion
        {
            public string ReferenceNumber { get; set; }
            public string Version { get; set; }
            public string ExpectedTitle { get; set; }
            public string ExpectedVersion { get; set; }
            public DateTime ExpectedVersionEarliestStartDate { get; set;}
            public DateTime ExpectedVersionLatestEndDate { get; set;}
        }

        private static readonly TestDataOptInStandardVersion[] TestDataForOptInStandardVersion =
        {
            new TestDataOptInStandardVersion
            {
                ReferenceNumber = "ST0001",
                Version = "1.0",
                ExpectedTitle = "Standard Title One",
                ExpectedVersion = "1.0",
                ExpectedVersionEarliestStartDate = DateTime.Now.AddDays(-10).Date,
                ExpectedVersionLatestEndDate = DateTime.Now.AddDays(10).Date
            }
        };
    }
}
