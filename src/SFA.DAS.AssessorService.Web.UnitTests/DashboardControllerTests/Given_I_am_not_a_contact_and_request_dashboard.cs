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
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Controllers;

namespace SFA.DAS.AssessorService.Web.UnitTests.DashboardControllerTests
{
    public class Given_I_am_not_a_contact_and_request_dashboard
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
        public async Task Then_show_start_page()
        {
            var actual = await _controller.Index();

            var actualViewResult = actual as RedirectToActionResult;
            actualViewResult.Should().NotBeNull();

            actualViewResult.ActionName.Should().Be("Index");
            actualViewResult.ControllerName.Should().Be("Home");
        }

        

    }    
}

