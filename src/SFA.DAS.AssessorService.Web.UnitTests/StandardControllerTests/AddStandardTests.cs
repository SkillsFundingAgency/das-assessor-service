using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.OrganisationStandards;
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
            modelState.AddModelError(nameof(AddStandardSearchViewModel.Search), "Error");
            modelState.SetModelValue(nameof(AddStandardSearchViewModel.Search), search, search);
            _sut.ViewData.ModelState.Merge(modelState);

            // Act
            var result = _sut.AddStandardSearch(search);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<ViewResult>(result);

                var viewResult = result as ViewResult;
                Assert.IsInstanceOf<AddStandardSearchViewModel>(viewResult.Model);

                var model = viewResult.Model as AddStandardSearchViewModel;
                Assert.AreEqual(search, model.Search);
            });
        }

        [Test]
        public void AddStandardSearch_RedirectsToGet_WhenPostCalledWithInvalidModelState()
        {
            // Arrange
            var search = "Te";
            var modelState = new ModelStateDictionary();
            modelState.AddModelError(nameof(AddStandardSearchViewModel.Search), "Error");
            modelState.SetModelValue(nameof(AddStandardSearchViewModel.Search), search, search);
            _sut.ViewData.ModelState.Merge(modelState);

            // Act
            var result = _sut.AddStandardSearch(new AddStandardSearchViewModel());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<RedirectToRouteResult>(result);

                var redirectToRouteResult = result as RedirectToRouteResult;
                Assert.AreEqual(StandardController.AddStandardSearchRouteGet, redirectToRouteResult.RouteName);
            });
        }

        [Test]
        public void AddStandardSearch_RedirectsToSearchResults_WhenPostCalledWithValidModelState()
        {
            // Arrange
            var modelState = new ModelStateDictionary();
            modelState.ClearValidationState(nameof(AddStandardSearchViewModel.Search));
            _sut.ViewData.ModelState.Merge(modelState);

            // Act
            var result = _sut.AddStandardSearch(new AddStandardSearchViewModel());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<RedirectToRouteResult>(result);

                var redirectToRouteResult = result as RedirectToRouteResult;
                Assert.AreEqual(StandardController.AddStandardSearchResultsRouteGet, redirectToRouteResult.RouteName);
            });
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
            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<ViewResult>(result);

                var viewResult = result as ViewResult;
                Assert.IsInstanceOf<AddStandardSearchViewModel>(viewResult.Model);

                var model = viewResult.Model as AddStandardSearchViewModel;
                Assert.AreEqual(search, model.Search);
                Assert.AreEqual(approvedStandards.Count, model.Approved.Count);
                Assert.AreEqual(allStandards.Count, model.Results.Count);
            });
        }

        [TestCase(null)]
        [TestCase("")]
        public void AddStandardSearchResults_ThrowsArgumentException_WhenCalledWithInvalidSearchParameter(string search)
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _sut.AddStandardSearchResults(search));
        }

        [Test]
        public async Task AddStandardChooseVersions_ReturnsViewWithPrePopulatedViewModel_WhenGetCalledWithInvalidModelState()
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
            modelState.AddModelError(nameof(AddStandardConfirmViewModel.SelectedVersions), "Error");
            modelState.SetModelValue(nameof(AddStandardConfirmViewModel.SelectedVersions), selectedVersions, selectedVersions);
            modelState.AddModelError(nameof(AddStandardConfirmViewModel.IsConfirmed), "Error");
            modelState.SetModelValue(nameof(AddStandardConfirmViewModel.IsConfirmed), isConfirmed, isConfirmed);

            _sut.ViewData.ModelState.Merge(modelState);

            // Act
            var result = await _sut.AddStandardChooseVersions(search, referenceNumber);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<ViewResult>(result);
                var viewResult = result as ViewResult;
                Assert.IsInstanceOf<AddStandardConfirmViewModel>(viewResult.Model);

                var model = viewResult.Model as AddStandardConfirmViewModel;
                Assert.AreEqual(search, model.Search);
                Assert.AreEqual(referenceNumber, model.StandardReference);
                Assert.AreEqual(standardVersions.Count, model.StandardVersions.Count);
                Assert.AreEqual(standardVersions.FirstOrDefault(), model.Standard);
                Assert.AreEqual(selectedVersions.Split(',').ToList(), model.SelectedVersions);
                Assert.AreEqual(bool.Parse(isConfirmed), model.IsConfirmed);
            });
        }

        [Test]
        public async Task AddStandardChooseVersions_ReturnsViewModel_WhenGetCalledWithValidModelState()
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
            var result = await _sut.AddStandardChooseVersions(search, referenceNumber);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<ViewResult>(result);
                var viewResult = result as ViewResult;
                Assert.IsInstanceOf<AddStandardConfirmViewModel>(viewResult.Model);

                var model = viewResult.Model as AddStandardConfirmViewModel;
                Assert.AreEqual(search, model.Search);
                Assert.AreEqual(referenceNumber, model.StandardReference);
                Assert.AreEqual(standardVersions.Count, model.StandardVersions.Count);
                Assert.AreEqual(standardVersions.FirstOrDefault(), model.Standard);
                Assert.AreEqual(new List<string>(), model.SelectedVersions);
                Assert.AreEqual(false, model.IsConfirmed);
            });
        }

        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("", null)]
        [TestCase(null, "")]
        public void AddStandardChooseVersions_ThrowsArugumentException_WhenGetCalledWithInvalidParameters(string search, string referenceNumber)
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _sut.AddStandardChooseVersions(search, referenceNumber));
        }

        [Test]
        public void AddStandardChooseVersions_RedirectsToGet_WhenPostCalledWithInvalidModelState()
        {
            // Arrange
            var modelState = new ModelStateDictionary();
            modelState.AddModelError(nameof(AddStandardConfirmViewModel.IsConfirmed), "Error");
            modelState.SetModelValue(nameof(AddStandardConfirmViewModel.Search), "false", "false");
            _sut.ViewData.ModelState.Merge(modelState);

            // Act
            var result = _sut.AddStandardChooseVersions(new AddStandardConfirmViewModel());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<RedirectToRouteResult>(result);

                var redirectToRouteResult = result as RedirectToRouteResult;
                Assert.AreEqual(StandardController.AddStandardChooseVersionsRouteGet, redirectToRouteResult.RouteName);
            });
        }

        [Test]
        public void AddStandardChooseVersions_RedirectsToAddStandardConfirm_WhenPostCalledWithInvalidModelState()
        {
            // Arrange
            var modelState = new ModelStateDictionary();
            modelState.ClearValidationState(nameof(AddStandardSearchViewModel.Search));
            _sut.ViewData.ModelState.Merge(modelState);

            // Act
            var result = _sut.AddStandardChooseVersions(new AddStandardConfirmViewModel());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<RedirectToRouteResult>(result);

                var redirectToRouteResult = result as RedirectToRouteResult;
                Assert.AreEqual(StandardController.AddStandardConfirmRouteGet, redirectToRouteResult.RouteName);
            });
        }

        [TestCase(null, null, "1.0")]
        [TestCase("", "", "1.0")]
        [TestCase("", null, "1.0")]
        [TestCase(null, "", "1.0")]
        [TestCase("Tech", "ST0001", null)]
        [TestCase("Tech", "ST0001", "")]
        public void AddStandardConfirm_ThrowsArugumentException_WhenGetCalledWithInvalidParameters(string search, string referenceNumber, string version)
        {
            // Arrange
            var versions = new List<string>();
            if(!string.IsNullOrEmpty(version))
            {
                versions.Add(version);
            }
            
            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _sut.AddStandardConfirm(search, referenceNumber, versions));
        }

        [Test]
        public async Task AddStandardConfirm_ReturnsView_WhenGetCalledWithValidParameters()
        {
            // Arrange
            var standardVersions = new List<StandardVersion> { new StandardVersion { IFateReferenceNumber = "ST0001" } };
            _mockStandardVersionApiClient.Setup(svc => svc.GetStandardVersionsByIFateReferenceNumber(It.IsAny<string>())).ReturnsAsync(standardVersions);

            // Act
            var result = await _sut.AddStandardConfirm("Tech", "ST0001", new List<string> { "1.0" });

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task AddStandardConfirm_SetsModelCorrectly_WhenGetCalledWithValidParameters()
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
            var result = await _sut.AddStandardConfirm("Tech", "ST0001", new List<string> { "1.0" });

            // Assert
            Assert.Multiple(() =>
            {
                var viewResult = result as ViewResult;
                Assert.IsNotNull(viewResult);

                var model = viewResult.Model as AddStandardConfirmViewModel;
                Assert.IsNotNull(model);
                Assert.AreEqual("Tech", model.Search);
                Assert.AreEqual("ST0001", model.StandardReference);
                Assert.AreEqual(standardVersions.First(), model.Standard);
            });
        }

        [Test]
        public async Task AddStandardConfirm_ReturnsRedirectToRouteResult_WhenGetCalledWithValidViewModel()
        {
            // Arrange
            var model = new AddStandardConfirmViewModel();
            var org = new EpaOrganisation { OrganisationId = Guid.NewGuid().ToString() };
            _mockOrgApiClient.Setup(x => x.GetEpaOrganisation(It.IsAny<string>())).ReturnsAsync(org);

            // Act
            var result = await _sut.AddStandardConfirm(model);

            // Assert
            Assert.IsInstanceOf<RedirectToRouteResult>(result);
        }

        [Test]
        public async Task AddStandardConfirm_CallsAddOrganisationStandardWithCorrectParameters_WhenGetCalledWithValidViewModel()
        {
            // Arrange
            var model = new AddStandardConfirmViewModel
            {
                Standard = new StandardVersion { IFateReferenceNumber = "ST0001"},
                SelectedVersions = new List<string> { "1.0" }
            };

            var org = new EpaOrganisation { OrganisationId = Guid.NewGuid().ToString() };

            _mockOrgApiClient.Setup(x => x.GetEpaOrganisation(It.IsAny<string>())).ReturnsAsync(org);

            // Act
            await _sut.AddStandardConfirm(model);

            // Assert
            Assert.Multiple(() =>
            {
                _mockOrgApiClient.Verify(x => x.AddOrganisationStandard(It.Is<OrganisationStandardAddRequest>(request =>
                request.OrganisationId == org.OrganisationId &&
                request.StandardReference == model.StandardReference &&
                request.StandardVersions.SequenceEqual(model.SelectedVersions) &&
                request.ContactId == UserId)), Times.Once);
            });
        }

        [TestCase(null)]
        [TestCase("")]
        public void AddStandardConfirmation_ThrowsArgumentOutOfRangeException_WhenGetCalledWithInvalidReferenceNumber(string referenceNumber)
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _sut.AddStandardConfirmation(referenceNumber));
        }

        [Test]
        public async Task AddStandardConfirmation_ReturnsViewWithCorrectModel_WhenGetCalledWithValidReferenceNumber()
        {
            // Arrange
            var referenceNumber = "ST0001";
            var standardVersions = new List<StandardVersion> { new StandardVersion { Title = "Title", Version = "1.0" } };
            _mockStandardVersionApiClient.Setup(x => x.GetEpaoRegisteredStandardVersions(It.IsAny<string>(), referenceNumber)).ReturnsAsync(standardVersions);
            _mockConfig.SetupGet(x => x.FeedbackUrl).Returns("http://feedbackurl");

            // Act
            var result = await _sut.AddStandardConfirmation(referenceNumber);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<ViewResult>(result);
                var viewResult = result as ViewResult;

                Assert.IsInstanceOf<AddStandardConfirmationViewModel>(viewResult.Model);
                var model = viewResult.Model as AddStandardConfirmationViewModel;

                Assert.AreEqual(standardVersions.First().Title, model.StandardTitle);
                Assert.AreEqual(standardVersions.Select(x => x.Version).ToList(), model.StandardVersions);
                Assert.AreEqual("http://feedbackurl", model.FeedbackUrl);
            });
        }
    }
}

