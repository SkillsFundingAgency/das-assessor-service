﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Controllers;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Search
{
    [TestFixture]
    public class When_I_post_a_valid_search
    {
        private IActionResult _result;

        [SetUp]
        public void Arrange()
        {
            var mediator = new Mock<IMediator>();

            mediator.Setup(m =>
                m.Send(It.IsAny<LearnerSearchRequest>(),
                    new CancellationToken())).ReturnsAsync(new List<LearnerSearchResponse>
            {
                new LearnerSearchResponse(){FamilyName = "Smith", Standard = "Standard Name 20"}
            });

            var controller = new SearchController(mediator.Object);

            _result = controller.SearchLearners(new LearnerSearchRequest
            {
                Surname = "Smith",
                EpaOrgId = "EPA0001",
                Uln = 1111111111,
                Username = "user"
            }).Result;
        }

        [Test]
        public void Then_OK_should_be_returned()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void Then_model_should_contain_search_results()
        {
            ((OkObjectResult) _result).Value.Should().BeOfType<List<LearnerSearchResponse>>();
        }

        [Test]
        public void Then_search_results_should_be_correct()
        {
            var searchResults = ((OkObjectResult) _result).Value as List<LearnerSearchResponse>;
            searchResults.Count.Should().Be(1);
            searchResults.First().FamilyName.Should().Be("Smith");
            searchResults.First().Standard.Should().Be("Standard Name 20");
        }
    }
}