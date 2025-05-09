﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Search
{
    [TestFixture]
    public class When_I_get_a_frameworklearner
    {
        private IActionResult _result;
        private Guid _guid = Guid.NewGuid();
        private GetFrameworkLearnerResponse certificate;

        [SetUp]
        [MoqAutoData]
        public void Arrange()
        {
            var mediator = new Mock<IMediator>();

            certificate = Builder<GetFrameworkLearnerResponse>.CreateNew().With(c => c.Id = _guid).Build();

            mediator.Setup(m =>
                m.Send(It.IsAny<GetFrameworkLearnerRequest>(),
                    new CancellationToken())).ReturnsAsync(certificate);

            var controller = new LearnerDetailsQueryController(mediator.Object);

            _result = controller.GetFrameworkLearner(_guid).Result;
        }

        [Test]
        public void Then_OK_should_be_returned()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void Then_model_should_contain_framework_certificate()
        {
            ((OkObjectResult) _result).Value.Should().BeOfType<GetFrameworkLearnerResponse>();
        }

        [Test]
        public void Then_framework_certificate_details_should_be_correct()
        {
            var certificateResult = ((OkObjectResult) _result).Value as GetFrameworkLearnerResponse;
            certificateResult.Should().BeEquivalentTo(certificate);
        }

    }
}