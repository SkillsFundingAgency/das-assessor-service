namespace SFA.DAS.AssessorService.Web.UnitTests.StandardControllerTests
{
    using Moq;
    using NUnit.Framework;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using SFA.DAS.AssessorService.Web.ViewModels.Standard;
    using SFA.DAS.AssessorService.Web.Controllers.Apply;
    using SFA.DAS.AssessorService.Api.Types.Models.Standards;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;
    using System;

    [TestFixture]
    public class AddStandardTests : StandardControllerTestBase
    {
        [SetUp]
        public void SetUp()
        {
            base.Arrange();
        }

        [Test]
        public void AddStandardSearch_ReturnsViewWithPrePopulatedViewModel_WhenGetCalledWithInvalidModelState()
        {
            // Arrange
            var search = "Te";
            var modelState = new ModelStateDictionary();
            modelState.AddModelError(nameof(AddStandardViewModel.StandardToFind), "Error");
            modelState.SetModelValue(nameof(AddStandardViewModel.StandardToFind), search, search);
            _sut.ViewData.ModelState.Merge(modelState);

            // Act
            var result = _sut.AddStandardSearch(search);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);

            var viewResult = result as ViewResult;
            Assert.IsInstanceOf<AddStandardViewModel>(viewResult.Model);

            var model = viewResult.Model as AddStandardViewModel;
            Assert.AreEqual(search, model.StandardToFind);
        }

        [Test]
        public void AddStandardSearch_RedirectsToGet_WhenPostCalledWithInvalidModelState()
        {
            // Arrange
            var search = "Te";
            var modelState = new ModelStateDictionary();
            modelState.AddModelError(nameof(AddStandardViewModel.StandardToFind), "Error");
            modelState.SetModelValue(nameof(AddStandardViewModel.StandardToFind), search, search);
            _sut.ViewData.ModelState.Merge(modelState);

            // Act
            var result = _sut.AddStandardSearch(new AddStandardViewModel());

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);

            var redirectToActionResult = result as RedirectToActionResult;
            Assert.AreEqual(nameof(StandardController.AddStandardSearch), redirectToActionResult.ActionName);
        }

        [Test]
        public void AddStandardSearch_RedirectsToSearchResults_WhenPostCalledWithValidModelState()
        {
            // Arrange
            var modelState = new ModelStateDictionary();
            modelState.ClearValidationState(nameof(AddStandardViewModel.StandardToFind));
            _sut.ViewData.ModelState.Merge(modelState);

            // Act
            var result = _sut.AddStandardSearch(new AddStandardViewModel());

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);

            var redirectToActionResult = result as RedirectToActionResult;
            Assert.AreEqual(nameof(StandardController.AddStandardSearchResults), redirectToActionResult.ActionName);
        }

        [Test]
        public async Task AddStandardSearchResults_ReturnsViewWithViewModel_WhenCalled()
        {
            // Arrange
            var search = "standard";

            var allStandards = new List<StandardVersion> {
                new StandardVersion { Title = "Standard 1", IFateReferenceNumber = "ST0001" },
                new StandardVersion { Title = "Standard 2", IFateReferenceNumber = "ST0002" }
            };

            var approvedStandards = new List<StandardVersion> {
                new StandardVersion { Title = "Standard 1", IFateReferenceNumber = "ST0001" }
            };

            _mockStandardVersionApiClient.Setup(c => c.GetLatestStandardVersions()).ReturnsAsync(allStandards);
            _mockStandardVersionApiClient.Setup(c => c.GetEpaoRegisteredStandardVersions(It.IsAny<string>())).ReturnsAsync(approvedStandards);

            // Act
            var result = await _sut.AddStandardSearchResults(search);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);

            var viewResult = result as ViewResult;
            Assert.IsInstanceOf<AddStandardViewModel>(viewResult.Model);

            var model = viewResult.Model as AddStandardViewModel;
            Assert.AreEqual(search, model.StandardToFind);
            Assert.AreEqual(approvedStandards.Count, model.Approved.Count);
            Assert.AreEqual(allStandards.Count, model.Results.Count);
        }

        [Test]
        public async Task AddStandardConfirmVersions_ReturnsViewWithPrePopulatedViewModel_WhenGetCalledWithInvalidModelState()
        {
            // Arrange
            var search = "Tec";
            var referenceNumber = "ST0001";
            
            var standardVersions = new List<StandardVersion> {
                new StandardVersion { Title = "Standard 1", IFateReferenceNumber = "ST0001", VersionEarliestStartDate = DateTime.Now.AddDays(-1) },
                new StandardVersion { Title = "Standard 2", IFateReferenceNumber = "ST0002", VersionEarliestStartDate = DateTime.Now }
            };

            _mockStandardVersionApiClient.Setup(c => c.GetStandardVersionsByIFateReferenceNumber(referenceNumber)).ReturnsAsync(standardVersions);

            var selectedVersions = "V1.0";
            var isConfirmed = false.ToString();

            var modelState = new ModelStateDictionary();
            modelState.AddModelError(nameof(AddStandardViewModel.SelectedVersions), "Error");
            modelState.SetModelValue(nameof(AddStandardViewModel.SelectedVersions), selectedVersions, selectedVersions);
            modelState.AddModelError(nameof(AddStandardViewModel.IsConfirmed), "Error");
            modelState.SetModelValue(nameof(AddStandardViewModel.IsConfirmed), isConfirmed, isConfirmed);
            
            _sut.ViewData.ModelState.Merge(modelState);

            // Act
            var result = await _sut.AddStandardConfirmVersions(search, referenceNumber);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsInstanceOf<AddStandardConfirmViewModel>(viewResult.Model);
            
            var model = viewResult.Model as AddStandardConfirmViewModel;
            Assert.AreEqual(search, model.StandardToFind);
            Assert.AreEqual(referenceNumber, model.StandardReference);
            Assert.AreEqual(standardVersions.Count, model.Results.Count);
            Assert.AreEqual(standardVersions.FirstOrDefault(), model.SelectedStandard);
            Assert.AreEqual(selectedVersions.Split(',').ToList(), model.SelectedVersions);
            Assert.AreEqual(bool.Parse(isConfirmed), model.IsConfirmed);
        }

        [Test]
        public async Task AddStandardConfirmVersions_ReturnsViewModel_WhenGetCalledWithValidModelState()
        {
            // Arrange
            var search = "Tec";
            var referenceNumber = "ST0001";

            var standardVersions = new List<StandardVersion> {
                new StandardVersion { Title = "Standard 1", IFateReferenceNumber = "ST0001", VersionEarliestStartDate = DateTime.Now.AddDays(-1) },
                new StandardVersion { Title = "Standard 2", IFateReferenceNumber = "ST0002", VersionEarliestStartDate = DateTime.Now }
            };

            _mockStandardVersionApiClient.Setup(c => c.GetStandardVersionsByIFateReferenceNumber(referenceNumber)).ReturnsAsync(standardVersions);

            var modelState = new ModelStateDictionary();
            _sut.ViewData.ModelState.Merge(modelState);

            // Act
            var result = await _sut.AddStandardConfirmVersions(search, referenceNumber);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsInstanceOf<AddStandardConfirmViewModel>(viewResult.Model);

            var model = viewResult.Model as AddStandardConfirmViewModel;
            Assert.AreEqual(search, model.StandardToFind);
            Assert.AreEqual(referenceNumber, model.StandardReference);
            Assert.AreEqual(standardVersions.Count, model.Results.Count);
            Assert.AreEqual(standardVersions.FirstOrDefault(), model.SelectedStandard);
            Assert.AreEqual(new List<string>(), model.SelectedVersions);
            Assert.AreEqual(false, model.IsConfirmed);
        }
    }
}

