using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Services;
using SFA.DAS.AssessorService.Web.ViewModels;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenIndexIsCalled
    {
        private OrganisationController _organisationController;
        private Mock<IOrganisationService> _organisationService;
        private Mock<ICache> _cache;

        [SetUp]
        public void Arrange()
        {
            _organisationService = new Mock<IOrganisationService>();
            _organisationService
                .Setup(serv => serv.GetOrganisation("jwt"))
                .Returns(Task.FromResult(new Organisation() { Id = "ID1" }));

            var httpContext = new Mock<IHttpContextAccessor>();
            httpContext
                .Setup(c => c.HttpContext)
                .Returns(new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim("ukprn", "12345"),
                        new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", "userid1")
                    }))
                });

            _cache = new Mock<ICache>();
            _cache
                .Setup(c => c.GetString("userid1"))
                .Returns("jwt");

            var logger = new Mock<ILogger<OrganisationController>>();

            _organisationController = new OrganisationController(_organisationService.Object, _cache.Object, httpContext.Object, logger.Object);
        }

        [Test]
        public void ThenTheControllerHasAnAuthorizeAttribute()
        {
            typeof(OrganisationController).Should().BeDecoratedWith<AuthorizeAttribute>();
        }

        [Test]
        public void ThenTheOrganisationServiceIsCalled()
        {
            _organisationController.Index().Wait();
            _organisationService.Verify(serv => serv.GetOrganisation("jwt"));
        }

        [Test]
        public void ThenTheJwtIsPulledFromTheCache()
        {
            _organisationController.Index().Wait();
            _cache.Verify(c => c.GetString("userid1"));
        }

        [Test]
        public void ThenAViewResultIsReturned()
        {
            var result = _organisationController.Index().Result;
            result.Should().BeOfType<ViewResult>();
        }

        [Test]
        public void ThenTheViewContainsAnOrganisationViewModel()
        {
            var result = _organisationController.Index().Result as ViewResult;
            result.Model.Should().BeOfType<Organisation>();
        }
    }
}