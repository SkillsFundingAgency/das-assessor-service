using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Controllers;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers
{
    [TestFixture]
    public class ControllerAuthorizeAttributeTests
    {

        private readonly List<string> _controllersThatDoNotRequireAuthorize = new List<string>()
        {
            "PingController"
        };

        [Test]
        public void ControllersShouldHaveAuthorizeAttribute()
        {
            var webAssembly = typeof(SearchController).GetTypeInfo().Assembly;

            var controllers = webAssembly.DefinedTypes.Where(c => c.BaseType == typeof(Controller)).ToList();

            foreach (var controller in controllers.Where(c => !_controllersThatDoNotRequireAuthorize.Contains(c.Name)))
            {
                controller.Should().BeDecoratedWith<AuthorizeAttribute>();
            }
        }
    }
}