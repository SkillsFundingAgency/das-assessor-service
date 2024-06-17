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
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Organisation;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationSearchControllerTests
{
    public class Given_I_request_the_search_page
    {
        private OrganisationSearchController _controller;
        private Mock<IContactsApiClient> _contactsApiClient;
        private Mock<IHttpContextAccessor> _mockContextAccessor;
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
                    GovUkIdentifier = GovIdentifier
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

            var organisationsApiClient = new Mock<IOrganisationsApiClient>();
            organisationsApiClient.Setup(o => o.SearchForOrganisations(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(
                Task.FromResult(
                    new Domain.Paging.PaginatedList<OrganisationSearchResult>(new List<OrganisationSearchResult> 
                    { 
                        new OrganisationSearchResult
                        {  
                            Id = "12345677",
                            TradingName = "Company A",
                            OrganisationIsLive = true,
                            Ukprn = 12345677
                        },
                        new OrganisationSearchResult
                        {
                            Id = "12345678",
                            TradingName = "Company B",
                            OrganisationIsLive = true,
                            Ukprn = 12345678
                        },
                        new OrganisationSearchResult
                        {
                            Id = "12345676",
                            TradingName = "Company C",
                            OrganisationIsLive = true,
                            Ukprn = 12345676
                        }
                    }, 3, 0, 10)));

            _controller = new OrganisationSearchController(
                Mock.Of<ILogger<OrganisationSearchController>>(),
                Mock.Of<IMapper>(),
                httpContextAccessor.Object,
                organisationsApiClient.Object,
                Mock.Of<IApplicationApiClient>(),
                _contactsApiClient.Object,
                new WebConfiguration(),
                Mock.Of<ISessionService>());

            _controller.TempData = new TempDataDictionary(httpContextAccessor.Object.HttpContext, Mock.Of<ITempDataProvider>());

        }


        [Test]
        public async Task Then_If_Results_Called_And_Organisation_Id_Is_Valid_Go_to_Dashboard()
        {
            _contactsApiClient
               .Setup(x => x.GetContactByGovIdentifier(It.IsAny<string>())).Returns(Task.FromResult(
               new ContactResponse
               {
                   Username = "Unknown-100",
                   Email = Email,
                   GovUkIdentifier = GovIdentifier,
                   OrganisationId = new Guid(),
                   Status = ContactStatus.Live,
               }));

            var orgSearchViewModel = new OrganisationSearchViewModel
            {
                Name = "name",
            };
            var actual = await _controller.Results(orgSearchViewModel, 0);

            var actualViewResult = actual as RedirectToActionResult;
            Assert.IsNotNull(actualViewResult);

            Assert.AreEqual("Index", actualViewResult.ActionName);
            Assert.AreEqual("Dashboard", actualViewResult.ControllerName);
        }

        [Test]
        public async Task Then_If_Results_Called_And_Search_String_Is_Empty()
        {
            var orgSearchViewModel = new OrganisationSearchViewModel();
            var actual = await _controller.Results(orgSearchViewModel, 0);

            var actualViewResult = actual as ViewResult;
            Assert.IsNotNull(actualViewResult);

            actualViewResult.ViewData.ModelState.ErrorCount.Should().BeGreaterThan(0);
            actualViewResult.ViewName.Should().Be("Index");
        }

        [Test]
        public async Task Then_If_Results_Called_And_Search_String_Not_Empty()
        {
            var orgSearchViewModel = new OrganisationSearchViewModel
            {
                Name = "name",
                SearchString = "company"
            };
            var actual = await _controller.Results(orgSearchViewModel, 0);

            var actualViewResult = actual as ViewResult;
            Assert.IsNotNull(actualViewResult);

            actualViewResult.Model.Should().BeOfType<OrganisationSearchViewModel>();
        }
    }    
}

