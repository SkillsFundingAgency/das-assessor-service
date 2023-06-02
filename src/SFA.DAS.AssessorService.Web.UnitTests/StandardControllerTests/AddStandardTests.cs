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
    using SFA.DAS.AssessorService.Api.Types.Models.AO;
    using SFA.DAS.AssessorService.Api.Types.Models.OrganisationStandards;

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
            modelState.AddModelError(nameof(AddStandardSearchViewModel.StandardToFind), "Error");
            modelState.SetModelValue(nameof(AddStandardSearchViewModel.StandardToFind), search, search);
            _sut.ViewData.ModelState.Merge(modelState);

            // Act
            var result = _sut.AddStandardSearch(search);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);

            var viewResult = result as ViewResult;
            Assert.IsInstanceOf<AddStandardSearchViewModel>(viewResult.Model);

            var model = viewResult.Model as AddStandardSearchViewModel;
            Assert.AreEqual(search, model.StandardToFind);
        }

        [Test]
        public void AddStandardSearch_RedirectsToGet_WhenPostCalledWithInvalidModelState()
        {
            // Arrange
            var search = "Te";
            var modelState = new ModelStateDictionary();
            modelState.AddModelError(nameof(AddStandardSearchViewModel.StandardToFind), "Error");
            modelState.SetModelValue(nameof(AddStandardSearchViewModel.StandardToFind), search, search);
            _sut.ViewData.ModelState.Merge(modelState);

            // Act
            var result = _sut.AddStandardSearch(new AddStandardSearchViewModel());

            // Assert
            Assert.IsInstanceOf<RedirectToRouteResult>(result);

            var redirectToRouteResult = result as RedirectToRouteResult;
            Assert.AreEqual(StandardController.AddStandardSearchRouteGet, redirectToRouteResult.RouteName);
        }

        [Test]
        public void AddStandardSearch_RedirectsToSearchResults_WhenPostCalledWithValidModelState()
        {
            // Arrange
            var modelState = new ModelStateDictionary();
            modelState.ClearValidationState(nameof(AddStandardSearchViewModel.StandardToFind));
            _sut.ViewData.ModelState.Merge(modelState);

            // Act
            var result = _sut.AddStandardSearch(new AddStandardSearchViewModel());

            // Assert
            Assert.IsInstanceOf<RedirectToRouteResult>(result);

            var redirectToRouteResult = result as RedirectToRouteResult;
            Assert.AreEqual(StandardController.AddStandardSearchResultsRouteGet, redirectToRouteResult.RouteName);
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
            Assert.IsInstanceOf<AddStandardSearchViewModel>(viewResult.Model);

            var model = viewResult.Model as AddStandardSearchViewModel;
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
            modelState.AddModelError(nameof(AddStandardSearchViewModel.SelectedVersions), "Error");
            modelState.SetModelValue(nameof(AddStandardSearchViewModel.SelectedVersions), selectedVersions, selectedVersions);
            modelState.AddModelError(nameof(AddStandardSearchViewModel.IsConfirmed), "Error");
            modelState.SetModelValue(nameof(AddStandardSearchViewModel.IsConfirmed), isConfirmed, isConfirmed);

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

        [Test]
        public void AddStandardConfirmVersions_RedirectsToGet_WhenPostCalledWithInvalidModelState()
        {
            // Arrange
            var modelState = new ModelStateDictionary();
            modelState.AddModelError(nameof(AddStandardSearchViewModel.IsConfirmed), "Error");
            modelState.SetModelValue(nameof(AddStandardSearchViewModel.StandardToFind), "false", "false");
            _sut.ViewData.ModelState.Merge(modelState);

            // Act
            var result = _sut.AddStandardConfirmVersions(new AddStandardConfirmViewModel());

            // Assert
            // Assert
            Assert.IsInstanceOf<RedirectToRouteResult>(result);

            var redirectToRouteResult = result as RedirectToRouteResult;
            Assert.AreEqual(StandardController.AddStandardConfirmVersionsRouteGet, redirectToRouteResult.RouteName);
        }

        [Test]
        public void AddStandardConfirmVersions_RedirectsToAddStandardConfirmOptIn_WhenPostCalledWithInvalidModelState()
        {
            // Arrange
            var modelState = new ModelStateDictionary();
            modelState.ClearValidationState(nameof(AddStandardSearchViewModel.StandardToFind));
            _sut.ViewData.ModelState.Merge(modelState);

            // Act
            var result = _sut.AddStandardConfirmVersions(new AddStandardConfirmViewModel());

            // Assert
            Assert.IsInstanceOf<RedirectToRouteResult>(result);

            var redirectToRouteResult = result as RedirectToRouteResult;
            Assert.AreEqual(StandardController.AddStandardConfirmOptInRouteGet, redirectToRouteResult.RouteName);
        }

        [Test]
        public void AddStandardConfirmOptIn_WhenSearchIsNull_ShouldThrowException()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _sut.AddStandardConfirmOptIn(null, "ST0001", new List<string> { "1.0" }));
        }

        [Test]
        public void AddStandardConfirmOptIn_WhenReferenceNumberIsNull_ShouldThrowException()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _sut.AddStandardConfirmOptIn("Tech", null, new List<string> { "1.0" }));
        }

        [Test]
        public void AddStandardConfirmOptIn_WhenVersionsIsNull_ShouldThrowException()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _sut.AddStandardConfirmOptIn("Tech", "ST0001", null));
        }

        [Test]
        public void AddStandardConfirmOptIn_WhenVersionsIsEmpty_ShouldThrowException()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _sut.AddStandardConfirmOptIn("Tech", "ST0001", new List<string>()));
        }


        [Test]
        public async Task AddStandardConfirmOptIn_WhenParametersAreValid_ReturnsView()
        {
            // Arrange
            var standardVersions = new List<StandardVersion> { new StandardVersion { IFateReferenceNumber = "ST0001" } };
            _mockStandardVersionApiClient.Setup(svc => svc.GetStandardVersionsByIFateReferenceNumber(It.IsAny<string>())).ReturnsAsync(standardVersions);

            // Act
            var result = await _sut.AddStandardConfirmOptIn("Tech", "ST0001", new List<string> { "1.0" });

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task AddStandardConfirmOptIn_WhenParametersAreValid_SetsModelCorrectly()
        {
            // Arrange
            var standardVersions = new List<StandardVersion> {
                new StandardVersion
                {
                    IFateReferenceNumber = "ST0001", Version = "1.0"
                },
                new StandardVersion
                {
                    IFateReferenceNumber = "ST0001", Version = "1.1"
                }
            };

            _mockStandardVersionApiClient.Setup(svc => svc.GetStandardVersionsByIFateReferenceNumber(It.IsAny<string>())).ReturnsAsync(standardVersions);

            // Act
            var result = await _sut.AddStandardConfirmOptIn("Tech", "ST0001", new List<string> { "1.0" });

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as AddStandardConfirmViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual("Tech", model.StandardToFind);
            Assert.AreEqual("ST0001", model.StandardReference);
            Assert.AreEqual(standardVersions.First(), model.SelectedStandard);
        }

        [Test]
        public async Task AddStandardConfirmOptIn_GivenValidViewModel_ReturnsRedirectToRouteResult()
        {
            // Arrange
            var model = new AddStandardConfirmViewModel();
            var org = new EpaOrganisation { OrganisationId = Guid.NewGuid().ToString() };
            _mockOrgApiClient.Setup(x => x.GetEpaOrganisation(It.IsAny<string>())).ReturnsAsync(org);

            // Act
            var result = await _sut.AddStandardConfirmOptIn(model);

            // Assert
            Assert.IsInstanceOf<RedirectToRouteResult>(result);
        }

        [Test]
        public async Task AddStandardConfirmOptIn_GivenValidViewModel_CallsAddOrganisationStandardWithCorrectParameters()
        {
            // Arrange
            var model = new AddStandardConfirmViewModel
            {
                StandardReference = "ref",
                SelectedVersions = new List<string> { "1.0" }
            };

            var org = new EpaOrganisation { OrganisationId = Guid.NewGuid().ToString() };

            _mockOrgApiClient.Setup(x => x.GetEpaOrganisation(It.IsAny<string>())).ReturnsAsync(org);

            // Act
            await _sut.AddStandardConfirmOptIn(model);

            // Assert
            _mockOrgApiClient.Verify(x => x.AddOrganisationStandard(It.Is<OrganisationStandardAddRequest>(request =>
                request.OrganisationId == org.OrganisationId &&
                request.StandardReference == model.StandardReference &&
                request.StandardVersions.SequenceEqual(model.SelectedVersions) &&
                request.ContactId == UserId)), Times.Once);
        }

        [TestCase(null)]
        [TestCase("")]
        public void AddStandardOptInConfirmation_GivenInvalidReferenceNumber_ThrowsArgumentOutOfRangeException(string referenceNumber)
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _sut.AddStandardOptInConfirmation(referenceNumber));
        }

        [Test]
        public async Task AddStandardOptInConfirmation_GivenValidReferenceNumber_ReturnsViewWithCorrectModel()
        {
            // Arrange
            var referenceNumber = "ST0001";
            var standardVersions = new List<StandardVersion> { new StandardVersion { Title = "Title", Version = "1.0" } };
            _mockStandardVersionApiClient.Setup(x => x.GetEpaoRegisteredStandardVersions(It.IsAny<string>(), referenceNumber)).ReturnsAsync(standardVersions);
            _mockConfig.SetupGet(x => x.FeedbackUrl).Returns("http://feedbackurl");

            // Act
            var result = await _sut.AddStandardOptInConfirmation(referenceNumber);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            
            Assert.IsInstanceOf<AddStandardOptInConfirmationViewModel>(viewResult.Model);
            var model = viewResult.Model as AddStandardOptInConfirmationViewModel;

            Assert.AreEqual(standardVersions.First().Title, model.StandardTitle);
            Assert.AreEqual(standardVersions.Select(x => x.Version).ToList(), model.Versions);
            Assert.AreEqual("http://feedbackurl", model.FeedbackUrl);
        }
    }
}

