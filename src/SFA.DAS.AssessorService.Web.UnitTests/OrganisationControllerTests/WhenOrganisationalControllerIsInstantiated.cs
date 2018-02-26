using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Controllers;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenOrganisationalControllerIsInstantiated : OrganisationControllerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            Setup();
        }
        
        [Test]
        public void Should_have_authorize_attribute()
        {
            typeof(OrganisationController).Should().BeDecoratedWith<AuthorizeAttribute>();
        }
    }
}