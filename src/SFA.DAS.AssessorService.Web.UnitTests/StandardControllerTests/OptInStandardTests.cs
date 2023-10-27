using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Domain.Exceptions;
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
                },
                new StandardVersion
                {
                    IFateReferenceNumber = "ST0002",
                    Title = "Standard Title Two",
                    Version = "2.0",
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
            Assert.Multiple(() =>
            {
                Assert.AreEqual(testData.ReferenceNumber, model.StandardReference);
                Assert.AreEqual(testData.ExpectedTitle, model.StandardTitle);
                Assert.AreEqual(testData.ExpectedVersion, model.Version);
                Assert.AreEqual(testData.ExpectedVersionEarliestStartDate, model.EffectiveFrom);
                Assert.AreEqual(testData.ExpectedVersionLatestEndDate, model.EffectiveTo);
            });
        }

        [TestCase(null)]
        [TestCase("")]
        public void OptInStandardVersion_ReturnsArgumentException_WhenGetCalledWithNullOrEmptyReference(string referenceNumber)
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.OptInStandardVersion(referenceNumber, "1.0"));
            Assert.That(ex.ParamName, Is.EqualTo("referenceNumber"));
        }

        [TestCase(null)]
        [TestCase("")]
        public void OptInStandardVersion_ReturnsArgumentException_WhenGetCalledWithNullOrEmptyVersion(string version)
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.OptInStandardVersion("ST0001", version));
            Assert.That(ex.ParamName, Is.EqualTo("version"));
        }

        [TestCase("ST0001", "1.3")]
        [TestCase("ST0002", "1.1")]
        public void OptInStandardVersion_ThrowsNotFoundException_WhenGetCalledWithNonExistentReferenceNumberAndVersionCombination(string referenceNumber, string version)
        {
            var ex = Assert.ThrowsAsync<NotFoundException>(async () => await _sut.OptInStandardVersion(referenceNumber, version));
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

            // Act
            var result = await _sut.OptInStandardVersion(model);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.TypeOf<RedirectToRouteResult>());

                var redirectResult = result as RedirectToRouteResult;
                Assert.That(redirectResult.RouteName, Is.EqualTo(StandardController.OptInStandardVersionConfirmationRouteGet));
                Assert.That(redirectResult.RouteValues["referenceNumber"], Is.EqualTo(model.StandardReference));
                Assert.That(redirectResult.RouteValues["version"], Is.EqualTo(model.Version));
            });
        }

        [Test]
        public async Task OptInStandardVersion_ReturnsArgumentException_WhenPostCalledWithNullModel()
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.OptInStandardVersion(null));
            Assert.That(ex.ParamName, Is.Null);
        }

        [TestCase(null)]
        [TestCase("")]
        public void OptInStandardVersion_ReturnsArgumentException_WhenPostCalledWithNullOrEmptyStandardReference(string standardReference)
        {
            var model = new OptInStandardVersionViewModel
            {
                StandardReference = standardReference,
                Version = "1.0"
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.OptInStandardVersion(model));
        }

        [TestCase(null)]
        [TestCase("")]
        public void OptInStandardVersion_ReturnsArgumentException_WhenPostCalledWithNullOrEmptyVersion(string version)
        {
            var model = new OptInStandardVersionViewModel
            {
                StandardReference = "ST0001",
                Version = version
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.OptInStandardVersion(model));
        }

        [Test]
        public async Task OptInStandardVersion_Redirects_To_OptInStandardVersionRouteGet_WhenModelStateIsInvalid()
        {
            var model = new OptInStandardVersionViewModel
            {
                StandardReference = "ST0001",
                Version = "1.1"
            };

            _sut.ModelState.AddModelError("some error", "some message");

            var result = await _sut.OptInStandardVersion(model);
            var redirectResult = result as RedirectToRouteResult;

            Assert.Multiple(() =>
            {
                Assert.That(redirectResult.RouteName, Is.EqualTo(nameof(StandardController.OptInStandardVersionRouteGet)));
                Assert.That(redirectResult.RouteValues["referenceNumber"], Is.EqualTo(model.StandardReference));
                Assert.That(redirectResult.RouteValues["version"], Is.EqualTo(model.Version));
            });
        }

        [Test]
        public async Task OptInStandardVersionConfirmation_ReturnsViewWithModel_WhenGetCalledWithValidReferenceAndVersion()
        {
            // Act
            var result = await _sut.OptInStandardVersionConfirmation("ST0001", "1.0");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.TypeOf<ViewResult>());
                var viewResult = result as ViewResult;
                Assert.That(viewResult.Model, Is.TypeOf<OptInStandardVersionConfirmationViewModel>());
                var model = viewResult.Model as OptInStandardVersionConfirmationViewModel;

                Assert.AreEqual("Standard Title One", model.StandardTitle);
                Assert.AreEqual("ST0001", model.StandardReference);
                Assert.AreEqual("1.0", model.Version);
                Assert.AreEqual("http://feedback-url.com", model.FeedbackUrl);
            });
        }

        [TestCase(null)]
        [TestCase("")]
        public void OptInStandardVersionConfirmation_ReturnsArgumentException_WhenGetCalledWithNullOrEmptyReference(string referenceNumber)
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.OptInStandardVersionConfirmation(referenceNumber, "1.0"));
            Assert.That(ex.ParamName, Is.EqualTo("referenceNumber"));
        }

        [TestCase(null)]
        [TestCase("")]
        public void OptInStandardVersionConfirmation_ReturnsArgumentException_WhenGetCalledWithNullOrEmptyVersion(string version)
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.OptInStandardVersionConfirmation("ST0001", version));
            Assert.That(ex.ParamName, Is.EqualTo("version"));
        }

        [Test]
        public void OptInStandardVersionConfirmation_ReturnsArgumentException_WhenGetCalledWithNonExistentReference()
        {
            var ex = Assert.ThrowsAsync<NotFoundException>(async () => await _sut.OptInStandardVersionConfirmation("ST0099", "1.0"));
        }

        public class TestDataOptInStandardVersion
        {
            public string ReferenceNumber { get; set; }
            public string Version { get; set; }
            public string ExpectedTitle { get; set; }
            public string ExpectedVersion { get; set; }
            public DateTime ExpectedVersionEarliestStartDate { get; set; }
            public DateTime ExpectedVersionLatestEndDate { get; set; }
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
