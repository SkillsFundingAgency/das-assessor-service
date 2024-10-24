using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using SFA.DAS.AssessorService.Web.ViewModels.Standard;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.StandardControllerTests
{
    [TestFixture]
    public class OptOutStandardTests : StandardControllerTestBase
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
        public async Task OptOutStandardVersion_ReturnsViewWithModel_WhenGetCalledWithValidReferenceAndVersion([ValueSource(nameof(TestDataForOptOutStandardVersion))] TestDataOptOutStandardVersion testData)
        {
            // Act
            var result = await _sut.OptOutStandardVersion(testData.ReferenceNumber, testData.Version);

            // Assert
            using (new AssertionScope())
            {
                Assert.That(result, Is.TypeOf<ViewResult>());
                var viewResult = result as ViewResult;
                Assert.That(viewResult.Model, Is.TypeOf<OptOutStandardVersionViewModel>());
                var model = viewResult.Model as OptOutStandardVersionViewModel;

                // Assert all fields are populated correctly
                model.StandardReference.Should().Be(testData.ReferenceNumber);
                model.StandardTitle.Should().Be(testData.ExpectedTitle);
                model.Version.Should().Be(testData.ExpectedVersion);
                model.EffectiveFrom.Should().Be(testData.ExpectedVersionEarliestStartDate);
                model.EffectiveTo.Should().Be(testData.ExpectedVersionLatestEndDate);
            }
        }

        [TestCase(null)]
        [TestCase("")]
        public void OptOutStandardVersion_ThrowsArgumentException_WhenGetCalledWithNullOrEmptyReference(string referenceNumber)
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.OptOutStandardVersion(referenceNumber, "1.0"));
            ex.ParamName.Should().Be("referenceNumber");
        }

        [TestCase(null)]
        [TestCase("")]
        public void OptOutStandardVersion_ThrowsArgumentException_WhenGetCalledWithNullOrEmptyVersion(string version)
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.OptOutStandardVersion("ST0001", version));
            ex.ParamName.Should().Be("version");
        }

        [TestCase("ST0001", "1.3")]
        [TestCase("ST0002", "1.1")]
        public void OptOutStandardVersion_ThrowsNotFoundException_WhenGetCalledWithNonExistentReferenceNumberAndVersionCombination(string referenceNumber, string version)
        {
            var ex = Assert.ThrowsAsync<NotFoundException>(async () => await _sut.OptOutStandardVersion(referenceNumber, version));
        }

        [Test]
        public async Task OptOutStandardVersion_RedirectsTo_OptOutStandardVersionConfirmationRouteGet_WhenPostCalledWithValidModel()
        {
            // Arrange
            var model = new OptOutStandardVersionViewModel
            {
                StandardReference = "ST0001",
                Version = "1.1",
                EffectiveFrom = DateTime.Today.AddDays(-5),
                EffectiveTo = DateTime.Today.AddDays(5)
            };

            // Act
            var result = await _sut.OptOutStandardVersion(model);

            using (new AssertionScope())
            {
                Assert.That(result, Is.TypeOf<RedirectToRouteResult>());

                var redirectResult = result as RedirectToRouteResult;
                redirectResult.RouteName.Should().Be(StandardController.OptOutStandardVersionConfirmationRouteGet);
                redirectResult.RouteValues["referenceNumber"].Should().Be(model.StandardReference);
                redirectResult.RouteValues["version"].Should().Be(model.Version);
            }
        }

        [Test, MoqAutoData]
        public async Task OptOutStandardVersion_Post_RedirectsTo_OptOutStandardVersionRouteGet_WhenModelIsInvalid(
            OptOutStandardVersionViewModel viewModel)
        {
            _sut.ModelState.AddModelError("some error name", "some error message");

            var result = await _sut.OptOutStandardVersion(viewModel);
            var redirectResult = result as RedirectToRouteResult;

            using (new AssertionScope())
            {
                redirectResult.RouteName.Should().Be(StandardController.OptOutStandardVersionRouteGet);
                redirectResult.RouteValues["referenceNumber"].Should().Be(viewModel.StandardReference);
                redirectResult.RouteValues["version"].Should().Be(viewModel.Version);
            }
        }

        [TestCase(null)]
        [TestCase("")]
        public void OptOutStandardVersion_ThrowsArgumentException_WhenPostCalledWithNullOrEmptyStandardReference(string standardReference)
        {
            var model = new OptOutStandardVersionViewModel
            {
                StandardReference = standardReference,
                Version = "1.0"
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.OptOutStandardVersion(model));
        }

        [TestCase(null)]
        [TestCase("")]
        public void OptPOutStandardVersion_ThrowsArgumentException_WhenPostCalledWithNullOrEmptyVersion(string version)
        {
            var model = new OptOutStandardVersionViewModel
            {
                StandardReference = "ST0001",
                Version = version
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.OptOutStandardVersion(model));
        }

        [Test]
        public async Task OptOutStandardVersionConfirmation_ReturnsViewWithModel_WhenGetCalledWithValidReferenceAndVersion()
        {
            // Act
            var result = await _sut.OptOutStandardVersionConfirmation("ST0001", "1.0");

            // Assert
            using (new AssertionScope())
            {
                Assert.That(result, Is.TypeOf<ViewResult>());
                var viewResult = result as ViewResult;
                Assert.That(viewResult.Model, Is.TypeOf<OptOutStandardVersionConfirmationViewModel>());
                var model = viewResult.Model as OptOutStandardVersionConfirmationViewModel;

                model.StandardTitle.Should().Be("Standard Title One");
                model.StandardReference.Should().Be("ST0001");
                model.Version.Should().Be("1.0");
                model.FeedbackUrl.Should().Be("http://feedback-url.com");
            }
        }

        [TestCase(null)]
        [TestCase("")]
        public void OptOutStandardVersionConfirmation_ThrowsArgumentException_WhenGetCalledWithNullOrEmptyReference(string referenceNumber)
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.OptOutStandardVersionConfirmation(referenceNumber, "1.0"));
            ex.ParamName.Should().Be("referenceNumber");
        }

        [TestCase(null)]
        [TestCase("")]
        public void OptOutStandardVersionConfirmation_ThrowsArgumentException_WhenGetCalledWithNullOrEmptyVersion(string version)
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.OptOutStandardVersionConfirmation("ST0001", version));
            ex.ParamName.Should().Be("version");
        }

        [Test]
        public void OptOutStandardVersionConfirmation_ThrowsArgumentException_WhenGetCalledWithNonExistentReference()
        {
            var ex = Assert.ThrowsAsync<NotFoundException>(async () => await _sut.OptOutStandardVersionConfirmation("ST0099", "1.0"));
        }


        public class TestDataOptOutStandardVersion
        {
            public string ReferenceNumber { get; set; }
            public string Version { get; set; }
            public string ExpectedTitle { get; set; }
            public string ExpectedVersion { get; set; }
            public DateTime ExpectedVersionEarliestStartDate { get; set; }
            public DateTime ExpectedVersionLatestEndDate { get; set; }
        }

        private static readonly TestDataOptOutStandardVersion[] TestDataForOptOutStandardVersion =
        {
            new TestDataOptOutStandardVersion
            {
                ReferenceNumber = "ST0001",
                Version = "1.0",
                ExpectedTitle = "Standard Title One",
                ExpectedVersion = "1.0",
                ExpectedVersionEarliestStartDate = DateTime.Now.AddDays(-10).Date,
                ExpectedVersionLatestEndDate = DateTime.Today
            }
        };
    }
}
