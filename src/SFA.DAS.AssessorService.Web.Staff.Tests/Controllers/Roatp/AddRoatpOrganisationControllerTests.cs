namespace SFA.DAS.AssessorService.Web.Staff.Tests.Controllers.Roatp
{
    using Api.Types.Models.Roatp;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using Staff.Controllers.Roatp;
    using Staff.Validators.Roatp;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Roatp;
    using System;
    using System.Security.Claims;
    using Api.Types.Models.Validation;
    using Microsoft.AspNetCore.Http;

    [TestFixture]
    public class AddRoatpOrganisationControllerTests
    {
        private Mock<IRoatpApiClient> _client;
        private Mock<ILogger<AddRoatpOrganisationController>> _logger;
        private Mock<IAddOrganisationValidator> _validator;
        private Mock<IRoatpSessionService> _sessionService;
        private AddRoatpOrganisationController _controller;

        [SetUp]
        public void Before_each_test()
        {
            _client = new Mock<IRoatpApiClient>();
            _logger = new Mock<ILogger<AddRoatpOrganisationController>>();
            _validator = new Mock<IAddOrganisationValidator>();
            _sessionService = new Mock<IRoatpSessionService>();
            
            _controller = new AddRoatpOrganisationController(_client.Object, _logger.Object, _validator.Object,
                _sessionService.Object);

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

        }

        [Test]
        public void Add_organisation_initialises_with_list_of_provider_types()
        {
            var providerTypes = new List<ProviderType>
            {
                new ProviderType {Id = 1, Type = "Main provider"},
                new ProviderType {Id = 2, Type = "Employer provider"}
            };
            _client.Setup(x => x.GetProviderTypes()).ReturnsAsync(providerTypes).Verifiable();

            var result = _controller.AddOrganisation(new AddOrganisationProviderTypeViewModel()).GetAwaiter().GetResult();

            result.Should().BeAssignableTo<ViewResult>();
            _client.VerifyAll();
        }

        [Test]
        public void Add_organisation_details_initialises_with_list_of_organisation_types()
        {
            var model = new AddOrganisationProviderTypeViewModel { ProviderTypeId = 1};

            var validationResult = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };
            _validator.Setup(x => x.ValidateOrganisationDetails(It.IsAny<AddOrganisationViewModel>()))
                .ReturnsAsync(validationResult);

            var organisationTypes = new List<OrganisationType>
            {
                new OrganisationType {Id = 1, Type = "Education"},
                new OrganisationType {Id = 2, Type = "Public sector body"}
            };
            _client.Setup(x => x.GetOrganisationTypes(It.IsAny<int>())).ReturnsAsync(organisationTypes).Verifiable();

            _sessionService.Setup(x => x.SetAddOrganisationDetails(It.IsAny<AddOrganisationViewModel>()));

            var result = _controller.AddOrganisationDetails(model).GetAwaiter().GetResult();

            result.Should().BeAssignableTo<ViewResult>();
            _client.VerifyAll();
        }
        
        [Test]
        public void Add_organisation_confirmation_shows_organisation_to_be_created()
        {
            var model = new AddOrganisationViewModel
            {
                ProviderTypeId = 1, UKPRN = "10001234", LegalName = "Legal Name",
                CompanyNumber = "12345678", OrganisationTypeId = 1          
            };

            var providerTypes = new List<ProviderType>
            {
                new ProviderType {Id = 1, Type = "Main provider"},
                new ProviderType {Id = 2, Type = "Employer provider"}
            };
            _client.Setup(x => x.GetProviderTypes()).ReturnsAsync(providerTypes).Verifiable();

            var organisationTypes = new List<OrganisationType>
            {
                new OrganisationType {Id = 1, Type = "Education"},
                new OrganisationType {Id = 2, Type = "Public sector body"}
            };

            var validationResult = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            _validator.Setup(x => x.ValidateOrganisationDetails(It.IsAny<AddOrganisationViewModel>()))
                .ReturnsAsync(validationResult);

            var result = _controller.AddOrganisationPreview(model).GetAwaiter().GetResult();

            var viewResult = result as ViewResult;
            var confirmationModel = viewResult.Model as AddOrganisationViewModel;

            confirmationModel.UKPRN.Should().Be(model.UKPRN);
        }

        [Test]
        public void Create_organisation_shows_error_message_if_unable_to_save_details()
        {
            var model = new AddOrganisationViewModel
            {
                ProviderTypeId = 1,
                UKPRN = "10001234",
                LegalName = "Legal Name",
                CompanyNumber = "12345678",
                OrganisationTypeId = 1
            };

            _client.Setup(x => x.CreateOrganisation(It.IsAny<CreateOrganisationRequest>())).ReturnsAsync(false);

            var result = _controller.CreateOrganisation(model).GetAwaiter().GetResult();

            var redirectResult = result as RedirectToActionResult;
            redirectResult.ActionName.Should().Be("Error");
            redirectResult.ControllerName.Should().Be("Home");
        }

        [Test]
        public void Create_organisation_redirects_to_home_page_on_successful_save_details()
        {
            var model = new AddOrganisationViewModel
            {
                ProviderTypeId = 1,
                UKPRN = "10001234",
                LegalName = "Legal Name",
                CompanyNumber = "12345678",
                OrganisationTypeId = 1
            };

            _client.Setup(x => x.CreateOrganisation(It.IsAny<CreateOrganisationRequest>())).ReturnsAsync(true);

            _controller.ControllerContext.HttpContext.User = CreateTestUser();

            var result = _controller.CreateOrganisation(model).GetAwaiter().GetResult();

            var viewResult = result as ViewResult;
            var successModel = viewResult.Model as BannerViewModel;

            successModel.CreateOrganisationCompanyName.Should().Be(model.LegalName.ToUpper());
        }

        [Test]
        public void Back_action_retrieves_model_state_from_session_and_passes_to_specified_action()
        {
            var temporaryModel = new AddOrganisationViewModel {UKPRN = "10002222", LegalName = "to be edited"};

            _sessionService.Setup(x => x.GetAddOrganisationDetails()).Returns(temporaryModel);

            var result = _controller.Back("nextAction", Guid.NewGuid()).GetAwaiter().GetResult();

            var redirectToActionResult = result as RedirectToActionResult;

            redirectToActionResult.Should().NotBeNull();
            redirectToActionResult.RouteValues["UKPRN"].Should().Be(temporaryModel.UKPRN);
        }

        private ClaimsPrincipal CreateTestUser()
        {
            var identities = new List<ClaimsIdentity>();

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", "Fred"));
            claims.Add(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname", "Bloggs"));

            var identity = new ClaimsIdentity(claims);
            identities.Add(identity);

            var principal = new ClaimsPrincipal(identities);

            return principal;
        }
    }
}
