using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Api.Controllers.Staff;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Search
{
    [TestFixture]
    public class When_I_post_a_valid_frameworksearch
    {
        private IActionResult _result;
        private Guid _guid = Guid.NewGuid();
        private string _frameworkName = "BEng Biochemical";
        private string _apprenticeshipLevelName = "Advanced";
        private string _certificationYear = "2002";

        [SetUp]
        public void Arrange()
        {
            var mediator = new Mock<IMediator>();

            mediator.Setup(m =>
                m.Send(It.IsAny<FrameworkLearnerSearchRequest>(),
                    new CancellationToken())).ReturnsAsync(new List<FrameworkLearnerSearchResponse>
            {
                new FrameworkLearnerSearchResponse(){Id = _guid, FrameworkName = _frameworkName, ApprenticeshipLevelName= _apprenticeshipLevelName, CertificationYear = _certificationYear }
            });

            var controller = new StaffSearchController(mediator.Object);

            _result = controller.StaffFrameworkSearch(new FrameworkLearnerSearchRequest
            {
                FirstName = "Ilya",
                LastName = "Vogel",
                DateOfBirth = DateTime.Now,
            }).Result;
        }

        [Test]
        public void Then_OK_should_be_returned()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void Then_model_should_contain_framework_search_results()
        {
            ((OkObjectResult) _result).Value.Should().BeOfType<List<FrameworkLearnerSearchResponse>>();
        }

        [Test]
        public void Then_framework_search_results_should_be_correct()
        {
            var searchResults = ((OkObjectResult) _result).Value as List<FrameworkLearnerSearchResponse>;
            searchResults.Count.Should().Be(1);
            searchResults.First().Id.Should().Be(_guid);
            searchResults.First().FrameworkName.Should().Be(_frameworkName);
            searchResults.First().ApprenticeshipLevelName.Should().Be(_apprenticeshipLevelName);
            searchResults.First().CertificationYear.Should().Be(_certificationYear);
        }
    }
}