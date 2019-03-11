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

            var result = _controller.AddOrganisation(new AddOrganisationViewModel()).GetAwaiter().GetResult();

            result.Should().BeAssignableTo<ViewResult>();
            _client.VerifyAll();
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(4)]
        public void Add_organisation_details_shows_validation_message_for_invalid_provider_type(int providerTypeId)
        {
            var model = new AddOrganisationViewModel {ProviderTypeId = providerTypeId};

            _validator.Setup(x => x.ValidateProviderType(It.IsAny<int>()))
                .Returns(new List<string> {"Invalid provider type"});

            var result = _controller.AddOrganisationDetails(model).GetAwaiter().GetResult();

            var viewResult = result as ViewResult;
            var validationModel = viewResult.Model as AddOrganisationViewModel;

            validationModel.ValidationErrors.Should().HaveCount(1);
        }

        [Test]
        public void Add_organisation_details_initialises_with_list_of_organisation_types()
        {
            var model = new AddOrganisationViewModel {ProviderTypeId = 1};

            _validator.Setup(x => x.ValidateProviderType(It.IsAny<int>())).Returns(new List<string>());

            var organisationTypes = new List<OrganisationType>
            {
                new OrganisationType {Id = 1, Type = "Education"},
                new OrganisationType {Id = 2, Type = "Public sector body"}
            };
            _client.Setup(x => x.GetOrganisationTypes(It.IsAny<int>())).ReturnsAsync(organisationTypes).Verifiable();

            _sessionService.Setup(x => x.GetAddOrganisationDetails(It.IsAny<Guid>())).Returns(model);
            _sessionService.Setup(x => x.SetAddOrganisationDetails(It.IsAny<AddOrganisationViewModel>()));

            var result = _controller.AddOrganisationDetails(model).GetAwaiter().GetResult();

            result.Should().BeAssignableTo<ViewResult>();
            _client.VerifyAll();
        }

        [Test]
        public void Add_organisation_details_resets_organisation_type_if_the_provider_type_has_been_changed()
        {
            var model = new AddOrganisationViewModel {ProviderTypeId = 1, OrganisationTypeId = 2};

            _validator.Setup(x => x.ValidateProviderType(It.IsAny<int>())).Returns(new List<string>());

            var organisationTypes = new List<OrganisationType>
            {
                new OrganisationType {Id = 1, Type = "Education"},
                new OrganisationType {Id = 2, Type = "Public sector body"}
            };
            _client.Setup(x => x.GetOrganisationTypes(It.IsAny<int>())).ReturnsAsync(organisationTypes).Verifiable();

            var previousVersionModel = new AddOrganisationViewModel {ProviderTypeId = 2};
            _sessionService.Setup(x => x.GetAddOrganisationDetails(It.IsAny<Guid>())).Returns(previousVersionModel);
            _sessionService.Setup(x => x.SetAddOrganisationDetails(It.IsAny<AddOrganisationViewModel>()));

            var result = _controller.AddOrganisationDetails(model).GetAwaiter().GetResult();

            var viewResult = result as ViewResult;
            var addOrganisationModel = viewResult.Model as AddOrganisationViewModel;
            addOrganisationModel.OrganisationTypeId.Should().Be(0);
            _client.VerifyAll();
        }

        [Test]
        public void Add_organisation_confirmation_shows_validation_messages_if_raised()
        {
            var model = new AddOrganisationViewModel {ProviderTypeId = 1, UKPRN = "", LegalName = "a"};

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

            _validator.Setup(x => x.ValidateOrganisationDetails(It.IsAny<AddOrganisationViewModel>()))
                .Returns(
                    new List<string>
                    {
                        "Invalid legal name",
                        "UKPRN is mandatory"
                    }
                );

            var result = _controller.AddOrganisationPreview(model).GetAwaiter().GetResult();

            var viewResult = result as ViewResult;
            var validationModel = viewResult.Model as AddOrganisationViewModel;

            validationModel.ValidationErrors.Should().HaveCount(2);
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

            _validator.Setup(x => x.ValidateProviderType(It.IsAny<int>())).Returns(new List<string>());

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

            var viewResult = result as ViewResult;
            var validationModel = viewResult.Model as AddOrganisationViewModel;

            validationModel.ValidationErrors.Should().HaveCount(1);
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

            successModel.CreateOrganisationCompanyName.Should().Be(model.LegalName);
        }

        [Test]
        public void Back_action_retrieves_model_state_from_session_and_passes_to_specified_action()
        {
            var temporaryModel = new AddOrganisationViewModel {UKPRN = "10002222", LegalName = "to be edited"};

            _sessionService.Setup(x => x.GetAddOrganisationDetails(It.IsAny<Guid>())).Returns(temporaryModel);

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
