using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Web.UnitTests.DashboardControllerTests
{
    public class Given_that_I_am_a_contact_and_request_dashboard
    {
        private DashboardController _controller;
        private Mock<IContactsApiClient> _contactsApiClient;
        private Mock<IOrganisationsApiClient> _organisationsApiClient;
        private const string Email = "test@test.com";
        private const string GovIdentifier = "GovIdentifier-12345";
       
        [SetUp]
        public void Arrange()
        {
            _contactsApiClient = new Mock<IContactsApiClient>();

            _contactsApiClient
                .Setup(x => x.GetContactByGovIdentifier(It.IsAny<string>())).Returns(Task.FromResult(
                new ContactResponse
                {
                    Username = "Unknown-100",
                    Email = Email,
                    GovUkIdentifier = GovIdentifier,
                    Status = ContactStatus.Live
                }));

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new List<ClaimsIdentity>
                {
                    new ( new List<Claim>
                    {
                        new(ClaimTypes.Email, Email),
                        new(ClaimTypes.NameIdentifier, GovIdentifier),
                    })
                })
            });

            _organisationsApiClient = new Mock<IOrganisationsApiClient>();

            _organisationsApiClient.Setup(x => x.GetEpaOrganisationById(It.IsAny<string>())).Returns(Task.FromResult(
                new EpaOrganisation
                {
                    Id = new Guid(),
                    Status = ContactStatus.Live
                }
                ));

            _controller = new DashboardController(
                httpContextAccessor.Object,
                Mock.Of<IApplicationApiClient>(),
                _contactsApiClient.Object,
                _organisationsApiClient.Object,
                Mock.Of<IDashboardApiClient>(),
                new WebConfiguration(),
                Mock.Of<ILogger<DashboardController>>());

            _controller.TempData = new TempDataDictionary(httpContextAccessor.Object.HttpContext, Mock.Of<ITempDataProvider>());

        }

        [Test]
        public async Task When_user_status_is_applying_show_applications()
        {
            _contactsApiClient
                .Setup(x => x.GetContactByGovIdentifier(It.IsAny<string>())).Returns(Task.FromResult(
                new ContactResponse
                {
                    Username = "Unknown-100",
                    Email = Email,
                    GovUkIdentifier = GovIdentifier,
                    Status = ContactStatus.Applying,
                }));

            var actual = await _controller.Index();

            var actualViewResult = actual as RedirectToActionResult;
            actualViewResult.Should().NotBeNull();

            actualViewResult.ActionName.Should().Be("Applications");
            actualViewResult.ControllerName.Should().Be("Application");
        }

        [Test]
        public async Task When_organisation_status_is_applying_show_applications()
        {
            _organisationsApiClient.Setup(x => x.GetEpaOrganisationById(It.IsAny<string>())).Returns(Task.FromResult(
                new EpaOrganisation
                {
                    Id = new Guid(),
                    Status = ContactStatus.Applying
                }
                ));

            var actual = await _controller.Index();

            var actualViewResult = actual as RedirectToActionResult;
            actualViewResult.Should().NotBeNull();

            actualViewResult.ActionName.Should().Be("Applications");
            actualViewResult.ControllerName.Should().Be("Application");
        }


    [Test]
        public async Task When_user_invite_is_pending_show_invite_pending()
        {
            _contactsApiClient
                .Setup(x => x.GetContactByGovIdentifier(It.IsAny<string>())).Returns(Task.FromResult(
                new ContactResponse
                {
                    Username = "Unknown-100",
                    Email = Email,
                    GovUkIdentifier = GovIdentifier,
                    Status = ContactStatus.InvitePending,
                    EndPointAssessorOrganisationId = null
                }));

            var actual = await _controller.Index();

            var actualViewResult = actual as RedirectToActionResult;
            actualViewResult.Should().NotBeNull();

            actualViewResult.ActionName.Should().Be("InvitePending");
            actualViewResult.ControllerName.Should().Be("Home");
        }

        [Test]
        public async Task When_EPAOId_is_null_show_not_registered()
        {
            _contactsApiClient
               .Setup(x => x.GetContactByGovIdentifier(It.IsAny<string>())).Returns(Task.FromResult(
               new ContactResponse
               {
                   Username = "Unknown-100",
                   Email = Email,
                   GovUkIdentifier = GovIdentifier,
                   OrganisationId = new Guid()
               }));

            var actual = await _controller.Index();

            var actualViewResult = actual as RedirectToActionResult;
            actualViewResult.Should().NotBeNull();

            actualViewResult.ActionName.Should().Be("NotRegistered");
            actualViewResult.ControllerName.Should().Be("Home");
        }

        [Test]
        public async Task When_organisation_is_not_live_show_applications()
        {
            _contactsApiClient
                   .Setup(x => x.GetContactByGovIdentifier(It.IsAny<string>())).Returns(Task.FromResult(
                   new ContactResponse
                   {
                       Username = "Unknown-100",
                       Email = Email,
                       GovUkIdentifier = GovIdentifier,
                       EndPointAssessorOrganisationId = "new" 
                   }));

            _organisationsApiClient.Setup(x => x.GetEpaOrganisationById(It.IsAny<string>())).Returns(Task.FromResult(
                new EpaOrganisation
                {
                    Id = new Guid(),
                    Status = ContactStatus.Applying
                }
                ));

            var actual = await _controller.Index();

            var actualViewResult = actual as RedirectToActionResult;
            actualViewResult.Should().NotBeNull();

            actualViewResult.ActionName.Should().Be("Applications");
            actualViewResult.ControllerName.Should().Be("Application");
        }


        [Test]
        public async Task When_user_has_no_organisation_show_organisation_search()
        {
            _organisationsApiClient.Setup(x => x.GetEpaOrganisationById(It.IsAny<string>())).Returns(Task.FromResult<EpaOrganisation>(null));

            var actual = await _controller.Index();

            var actualViewResult = actual as RedirectToActionResult;
            actualViewResult.Should().NotBeNull();

            actualViewResult.ActionName.Should().Be("Index");
            actualViewResult.ControllerName.Should().Be("OrganisationSearch");
        }



    }    
}

