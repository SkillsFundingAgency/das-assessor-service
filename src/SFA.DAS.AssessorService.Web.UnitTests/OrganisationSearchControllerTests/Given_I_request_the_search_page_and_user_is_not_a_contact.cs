using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Infrastructure;

using SFA.DAS.AssessorService.Web.ViewModels.Organisation;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationSearchControllerTests
{
    public class Given_I_request_the_search_page_and_user_is_not_contact
    {
        private OrganisationSearchController _controller;
        private Mock<IContactsApiClient> _contactsApiClient;
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

            _controller = new OrganisationSearchController(
                Mock.Of<ILogger<OrganisationSearchController>>(),
                Mock.Of<IMapper>(),
                httpContextAccessor.Object,
                Mock.Of<IOrganisationsApiClient>(),
                Mock.Of<IApplicationApiClient>(),
                _contactsApiClient.Object,
                new WebConfiguration(),
                Mock.Of<ISessionService>());
        }

    
        [Test]
        public async Task Then_Results_redirects_to_Home()
        {
            var orgSearchViewModel = new OrganisationSearchViewModel
            {
                Name = "name",
            };
            var actual = await _controller.Results(orgSearchViewModel,0);

            var actualViewResult = actual as RedirectToActionResult;
            actualViewResult.Should().NotBeNull();

            actualViewResult.ActionName.Should().Be("Index");
            actualViewResult.ControllerName.Should().Be("Home");
        }

        





    }    
}

