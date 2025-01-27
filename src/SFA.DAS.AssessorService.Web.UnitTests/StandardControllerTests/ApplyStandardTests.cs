using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.ViewModels.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.StandardControllerTests
{
    [TestFixture]
    public class ApplyStandardTests : StandardControllerTestBase
    {
        [SetUp]
        public void SetUp()
        {
            base.Arrange();
        }

        [Test]
        public void ApplyStandardSearch_ReturnsViewWithPrePopulatedViewModel_WhenGetCalledWithInvalidModelState()
        {
            // Arrange
            var search = "Te";
            var modelState = new ModelStateDictionary();
            modelState.AddModelError(nameof(ApplyStandardSearchViewModel.Search), "Error");
            modelState.SetModelValue(nameof(ApplyStandardSearchViewModel.Search), search, search);
            _sut.ViewData.ModelState.Merge(modelState);

            // Act
            var result = _sut.ApplyStandardSearch(Guid.NewGuid(), search);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ViewResult>();

                var viewResult = result as ViewResult;
                viewResult.Model.Should().BeOfType<ApplyStandardSearchViewModel>();

                var model = viewResult.Model as ApplyStandardSearchViewModel;
                model.Search.Should().Be(search);
            }
        }

        [Test]
        public void ApplyStandardSearch_RedirectsToGet_WhenPostCalledWithInvalidModelState()
        {
            // Arrange
            var search = "Te";
            var modelState = new ModelStateDictionary();
            modelState.AddModelError(nameof(ApplyStandardSearchViewModel.Search), "Error");
            modelState.SetModelValue(nameof(ApplyStandardSearchViewModel.Search), search, search);
            _sut.ViewData.ModelState.Merge(modelState);

            // Act
            var result = _sut.ApplyStandardSearch(new ApplyStandardSearchViewModel { Id = Guid.NewGuid() });

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<RedirectToRouteResult>();

                var redirectToRouteResult = result as RedirectToRouteResult;
                redirectToRouteResult.RouteName.Should().Be(StandardController.ApplyStandardSearchRouteGet);
            }
        }

        [Test]
        public void ApplyStandardSearch_RedirectsToSearchResults_WhenPostCalledWithValidModelState()
        {
            // Arrange
            var modelState = new ModelStateDictionary();
            modelState.ClearValidationState(nameof(ApplyStandardSearchViewModel.Search));
            _sut.ViewData.ModelState.Merge(modelState);

            // Act
            var result = _sut.ApplyStandardSearch(new ApplyStandardSearchViewModel { Id = Guid.NewGuid() });

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<RedirectToRouteResult>();

                var redirectToRouteResult = result as RedirectToRouteResult;
                redirectToRouteResult.RouteName.Should().Be(StandardController.ApplyStandardSearchResultsRouteGet);
            }
        }

        [TestCase]
        public void ApplyStandardSearchResults_ThrowsArgumentException_WhenGetCalledWithInvalidIdParameter()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _sut.ApplyStandardSearchResults(Guid.Empty, string.Empty));
        }

        [TestCase(null)]
        [TestCase("")]
        public void ApplyStandardSearchResults_ThrowsArgumentException_WhenGetCalledWithInvalidSearchParameter(string search)
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _sut.ApplyStandardSearchResults(Guid.NewGuid(), search));
        }

        [Test]
        public async Task ApplyStandardSearchResults_ReturnsViewWithViewModel_WhenGetCalled()
        {
            // Arrange
            var search = "standard";

            var standards = new List<StandardVersion> {
                new StandardVersion { Title = "Standard 1", IFateReferenceNumber = "ST0001" },
                new StandardVersion { Title = "Standard 2", IFateReferenceNumber = "ST0002" }
            };

            _mockStandardVersionApiClient.Setup(c => c.GetLatestStandardVersions()).ReturnsAsync(standards);

            // Act
            var result = await _sut.ApplyStandardSearchResults(Guid.NewGuid(), search);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ViewResult>();

                var viewResult = result as ViewResult;
                viewResult.Model.Should().BeOfType<ApplyStandardSearchViewModel>();

                var model = viewResult.Model as ApplyStandardSearchViewModel;
                model.Search.Should().Be(search);
                model.Results.Count.Should().Be(standards.Count);
            }
        }

        [TestCase]
        public void ApplyStandardConfirm_ThrowsArgumentException_WhenGetCalledWithInvalidIdParameter()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _sut.ApplyStandardConfirm(Guid.Empty, string.Empty, string.Empty));
        }

        [TestCase(null, null)]
        [TestCase("", null)]
        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase("Tec", null)]
        [TestCase("Tec", "")]
        public void ApplyStandardConfirm_ThrowsArgumentException_WhenGetCalledWithInvalidParameters(string search, string referenceNumber)
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _sut.ApplyStandardConfirm(Guid.NewGuid(), search, referenceNumber));
        }

        [Test]
        public async Task ApplyStandardConfirm_RedirectsToStandardDetails_WhenGetCalledForStandardWithApprovedVersions()
        {
            // Arrange
            var appliedStandardVersions = new List<AppliedStandardVersion>
            {
                new AppliedStandardVersion { IFateReferenceNumber = "ST0001", ApprovedStatus = ApprovedStatus.Approved, Version = "1.0" },
                new AppliedStandardVersion { IFateReferenceNumber = "ST0001", ApprovedStatus = ApprovedStatus.NotYetApplied, Version = "1.1"}
            };

            _mockOrgApiClient.Setup(p => p.GetAppliedStandardVersionsForEPAO(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(appliedStandardVersions);

            // Act
            var result = await _sut.ApplyStandardConfirm(Guid.NewGuid(), "Tec", "ST0001");

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<RedirectToRouteResult>();

                var redirectToRouteResult = result as RedirectToRouteResult;
                redirectToRouteResult.RouteName.Should().Be(StandardController.StandardDetailsRouteGet);
            }
        }

        [Test]
        public async Task ApplyStandardConfirm_RedirectsToApplyStandardConfirmOfqual_WhenGetCalledForOfqualLatestStandardWithNonApprovedVersions()
        {
            // Arrange
            var appliedStandardVersions = new List<AppliedStandardVersion>
            {
                new AppliedStandardVersion { IFateReferenceNumber = "ST0001", ApprovedStatus = ApprovedStatus.ApplyInProgress, Version = "1.0", EqaProviderName = "Other" },
                new AppliedStandardVersion { IFateReferenceNumber = "ST0001", ApprovedStatus = ApprovedStatus.NotYetApplied, Version = "1.1", EqaProviderName = "Other" },
                new AppliedStandardVersion { IFateReferenceNumber = "ST0001", ApprovedStatus = ApprovedStatus.FeedbackAdded, Version = "1.2", EqaProviderName = "Other" },
                new AppliedStandardVersion { IFateReferenceNumber = "ST0001", ApprovedStatus = ApprovedStatus.Withdrawn, Version = "1.3", EqaProviderName = "Ofqual"}
            };

            _mockOrgApiClient.Setup(p => p.GetAppliedStandardVersionsForEPAO(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(appliedStandardVersions);

            // Act
            var result = await _sut.ApplyStandardConfirm(Guid.NewGuid(), "Tec", "ST0001");

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<RedirectToRouteResult>();

                var redirectToRouteResult = result as RedirectToRouteResult;
                redirectToRouteResult.RouteName.Should().Be(StandardController.ApplyStandardConfirmOfqualRouteGet);
            }
        }

        [Test]
        public async Task ApplyStandardConfirm_ReturnsViewModel_WhenGetCalledForNonOfqualLatestStandardWithNonApprovedVersions()
        {
            // Arrange
            var id = Guid.NewGuid();
            var search = "Tec";
            var referenceNumber = "ST0001";

            var versionEarliestStartDate = DateTime.Today.AddMonths(-1);

            var appliedStandardVersions = new List<AppliedStandardVersion>
            {
                new AppliedStandardVersion { IFateReferenceNumber = "ST0001", ApprovedStatus = ApprovedStatus.ApplyInProgress, Version = "1.0", EqaProviderName = "Other", VersionEarliestStartDate = versionEarliestStartDate },
                new AppliedStandardVersion { IFateReferenceNumber = "ST0001", ApprovedStatus = ApprovedStatus.ApplyInProgress, Version = "1.1", EqaProviderName = "Other" }
            };

            _mockOrgApiClient.Setup(p => p.GetAppliedStandardVersionsForEPAO(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(appliedStandardVersions);

            // Act
            var result = await _sut.ApplyStandardConfirm(id, search, referenceNumber);

            // Assert
            using (new AssertionScope()) {
                result.Should().BeOfType<ViewResult>();

                var viewResult = result as ViewResult;
                viewResult.Model.Should().BeOfType<ApplyStandardConfirmViewModel>();

                var model = viewResult.Model as ApplyStandardConfirmViewModel;
                model.Id.Should().Be(id);
                model.Search.Should().Be(search);
                model.StandardReference.Should().Be(referenceNumber);
                model.Results.Count.Should().Be(appliedStandardVersions.Count);
                model.SelectedStandard.Version.Should().Be("1.1");
                model.EarliestVersionEffectiveFrom.Should().Be(versionEarliestStartDate);
            }
        }

        [Test]
        public async Task ApplyStandardConfirm_ReturnsViewWithPrePopulatedViewModel_WhenGetCalledWithInvalidModelState()
        {
            // Arrange
            var id = Guid.NewGuid();
            var search = "Tec";
            var referenceNumber = "ST0001";

            var versionEarliestStartDate = DateTime.Today.AddMonths(-1);

            var appliedStandardVersions = new List<AppliedStandardVersion>
            {
                new AppliedStandardVersion { IFateReferenceNumber = "ST0001", ApprovedStatus = ApprovedStatus.ApplyInProgress, Version = "1.0", EqaProviderName = "Other", VersionEarliestStartDate = versionEarliestStartDate },
                new AppliedStandardVersion { IFateReferenceNumber = "ST0001", ApprovedStatus = ApprovedStatus.ApplyInProgress, Version = "1.1", EqaProviderName = "Other" }
            };

            _mockOrgApiClient.Setup(p => p.GetAppliedStandardVersionsForEPAO(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(appliedStandardVersions);

            var selectedVersions = "1.0";
            var isConfirmed = false.ToString();

            var modelState = new ModelStateDictionary();
            modelState.AddModelError(nameof(AddStandardConfirmViewModel.SelectedVersions), "Error");
            modelState.SetModelValue(nameof(AddStandardConfirmViewModel.SelectedVersions), selectedVersions, selectedVersions);
            modelState.AddModelError(nameof(AddStandardConfirmViewModel.IsConfirmed), "Error");
            modelState.SetModelValue(nameof(AddStandardConfirmViewModel.IsConfirmed), isConfirmed, isConfirmed);

            _sut.ViewData.ModelState.Merge(modelState);

            // Act
            var result = await _sut.ApplyStandardConfirm(id, search, referenceNumber);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ViewResult>();
                var viewResult = result as ViewResult;
                viewResult.Model.Should().BeOfType<ApplyStandardConfirmViewModel>();

                var model = viewResult.Model as ApplyStandardConfirmViewModel;
                model.Search.Should().Be(search);
                model.StandardReference.Should().Be(referenceNumber);
                model.Results.Count.Should().Be(appliedStandardVersions.Count);
                model.SelectedStandard.Version.Should().Be("1.1");
                model.SelectedVersions.Should().BeEquivalentTo(selectedVersions.Split(',').ToList());
                model.IsConfirmed.Should().Be(bool.Parse(isConfirmed));
            }
        }

        [Test]
        public async Task ApplyStandardConfirm_ReturnsViewModel_WhenGetCalledWithValidModelState()
        {
            // Arrange
            var id = Guid.NewGuid();
            var search = "Tec";
            var referenceNumber = "ST0001";

            var versionEarliestStartDate = DateTime.Today.AddMonths(-1);

            var appliedStandardVersions = new List<AppliedStandardVersion>
            {
                new AppliedStandardVersion { IFateReferenceNumber = "ST0001", ApprovedStatus = ApprovedStatus.ApplyInProgress, Version = "1.0", EqaProviderName = "Other", VersionEarliestStartDate = versionEarliestStartDate },
                new AppliedStandardVersion { IFateReferenceNumber = "ST0001", ApprovedStatus = ApprovedStatus.ApplyInProgress, Version = "1.1", EqaProviderName = "Other" }
            };

            _mockOrgApiClient.Setup(p => p.GetAppliedStandardVersionsForEPAO(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(appliedStandardVersions);

            // Act
            var result = await _sut.ApplyStandardConfirm(id, search, referenceNumber);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ViewResult>();
                var viewResult = result as ViewResult;
                viewResult.Model.Should().BeOfType<ApplyStandardConfirmViewModel>();

                var model = viewResult.Model as ApplyStandardConfirmViewModel;
                model.Search.Should().Be(search);
                model.StandardReference.Should().Be(referenceNumber);
                model.Results.Count.Should().Be(appliedStandardVersions.Count);
                model.SelectedStandard.Version.Should().Be("1.1");
                model.SelectedVersions.Should().BeEquivalentTo(new List<string>());
                model.IsConfirmed.Should().Be(false);
            }
        }

        [Test]
        public async Task ApplyStandardConfirm_RedirectsToGet_WhenPostCalledWithInvalidModelState()
        {
            // Arrange
            var modelState = new ModelStateDictionary();
            modelState.AddModelError(nameof(ApplyStandardConfirmViewModel.IsConfirmed), "Error");
            modelState.SetModelValue(nameof(ApplyStandardConfirmViewModel.IsConfirmed), "false", "false");
            _sut.ViewData.ModelState.Merge(modelState);

            // Act
            var result = await _sut.ApplyStandardConfirm(new ApplyStandardConfirmViewModel { Id = Guid.NewGuid(), Search = "Tec", StandardReference = "ST0001" });

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<RedirectToRouteResult>();

                var redirectToRouteResult = result as RedirectToRouteResult;
                redirectToRouteResult.RouteName.Should().Be(StandardController.ApplyStandardConfirmRouteGet);
            }
        }

        [Test]
        public async Task ApplyStandardConfirm_RedirectsToSequenceSignPost_WhenPostCalledWithValidModelState()
        {
            // Arrange
            var modelState = new ModelStateDictionary();
            modelState.ClearValidationState(nameof(ApplyStandardConfirmViewModel.IsConfirmed));
            modelState.ClearValidationState(nameof(ApplyStandardConfirmViewModel.SelectedVersions));
            _sut.ViewData.ModelState.Merge(modelState);

            var versionEarliestStartDate = DateTime.Today.AddMonths(-1);

            var appliedStandardVersions = new List<AppliedStandardVersion>
            {
                new AppliedStandardVersion { IFateReferenceNumber = "ST0001", ApprovedStatus = ApprovedStatus.ApplyInProgress, Version = "1.0", EqaProviderName = "Other", VersionEarliestStartDate = versionEarliestStartDate },
                new AppliedStandardVersion { IFateReferenceNumber = "ST0001", ApprovedStatus = ApprovedStatus.ApplyInProgress, Version = "1.1", EqaProviderName = "Other" }
            };

            _mockOrgApiClient.Setup(p => p.GetAppliedStandardVersionsForEPAO(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(appliedStandardVersions);

            _mockQnaApiClient.Setup(p => p.GetApplicationDataDictionary(It.IsAny<Guid>()))
                .ReturnsAsync(new Dictionary<string, object>());

            // Act
            var result = await _sut.ApplyStandardConfirm(new ApplyStandardConfirmViewModel { Id = Guid.NewGuid(), Search = "Tec", StandardReference = "ST0001" });

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<RedirectToActionResult>();

                var redirectToActionResult = result as RedirectToActionResult;
                redirectToActionResult.ActionName.Should().Be(nameof(ApplicationController.SequenceSignPost));
                redirectToActionResult.ControllerName.Should().Be(nameof(ApplicationController).RemoveController());
            }
        }

        [Test]
        public async Task ApplyStandardConfirm_UpdateApplicationDataDictionary_WhenPostCalledWithValidModelState()
        {
            var modelState = new ModelStateDictionary();
            modelState.ClearValidationState(nameof(ApplyStandardConfirmViewModel.IsConfirmed));
            modelState.ClearValidationState(nameof(ApplyStandardConfirmViewModel.SelectedVersions));
            _sut.ViewData.ModelState.Merge(modelState);

            var versionEarliestStartDate = DateTime.Today.AddMonths(-1);

            var appliedStandardVersions = new List<AppliedStandardVersion>
            {
                new AppliedStandardVersion { IFateReferenceNumber = "ST0001", ApprovedStatus = ApprovedStatus.ApplyInProgress, Version = "1.0", EqaProviderName = "Other", VersionEarliestStartDate = versionEarliestStartDate },
                new AppliedStandardVersion { IFateReferenceNumber = "ST0001", ApprovedStatus = ApprovedStatus.ApplyInProgress, Version = "1.1", EqaProviderName = "Office for Students" }
            };

            _mockOrgApiClient.Setup(p => p.GetAppliedStandardVersionsForEPAO(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(appliedStandardVersions);

            _mockQnaApiClient.Setup(p => p.GetApplicationDataDictionary(It.IsAny<Guid>()))
                .ReturnsAsync(new Dictionary<string, object>());

            // Act
            var result = await _sut.ApplyStandardConfirm(new ApplyStandardConfirmViewModel { Id = Guid.NewGuid(), Search = "Tec", StandardReference = "ST0001" });

            // Assert
            _mockQnaApiClient.Verify(
                x => x.UpdateApplicationDataDictionary(It.IsAny<Guid>(),
                    It.Is<Dictionary<string, object>>(d => d.ContainsKey(nameof(ApplicationData.Eqap)) && d[nameof(ApplicationData.Eqap)].Equals("Office for Students"))),
                Times.Once);
        }

        [Test]
        public async Task ApplyStandardConfirm_UpdateStandardData_WhenPostCalledWithValidModelState()
        {
            // Arrange
            var modelState = new ModelStateDictionary();
            modelState.ClearValidationState(nameof(ApplyStandardConfirmViewModel.IsConfirmed));
            modelState.ClearValidationState(nameof(ApplyStandardConfirmViewModel.SelectedVersions));
            _sut.ViewData.ModelState.Merge(modelState);

            var versionEarliestStartDate = DateTime.Today.AddMonths(-1);

            var appliedStandardVersions = new List<AppliedStandardVersion>
            {
                new AppliedStandardVersion { IFateReferenceNumber = "ST0001", LarsCode = 111, Title = "Some standard", ApprovedStatus = ApprovedStatus.ApplyInProgress, Version = "1.0", EqaProviderName = "Other", VersionEarliestStartDate = versionEarliestStartDate },
                new AppliedStandardVersion { IFateReferenceNumber = "ST0001", LarsCode = 111, Title = "Some standard", ApprovedStatus = ApprovedStatus.ApplyInProgress, Version = "1.1", EqaProviderName = "Office for Students" }
            };

            _mockOrgApiClient.Setup(p => p.GetAppliedStandardVersionsForEPAO(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(appliedStandardVersions);

            _mockQnaApiClient.Setup(p => p.GetApplicationDataDictionary(It.IsAny<Guid>()))
                .ReturnsAsync(new Dictionary<string, object>());

            // Act
            var id = Guid.NewGuid();
            var result = await _sut.ApplyStandardConfirm(new ApplyStandardConfirmViewModel 
            { 
                Id = id, Search = "Tec", StandardReference = "ST0001", SelectedVersions = new List<string> { "1.0" } 
            });

            // Assert
            _mockApiClient.Verify(
                x => x.UpdateStandardData(
                    id,
                    111,
                    "ST0001",
                    "Some standard",
                    new List<string> { "1.0" },
                    StandardApplicationTypes.Full),
                Times.Once
            );
        }
    }
}

