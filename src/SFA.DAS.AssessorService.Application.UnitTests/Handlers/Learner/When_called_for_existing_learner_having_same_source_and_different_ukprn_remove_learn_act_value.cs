﻿using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{
    [TestFixture]
    public class When_called_for_existing_learner_having_same_source_and_different_ukprn_remove_learn_act_value : ImportLearnerDetailHandlerTestsBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();
        }

        [Test]
        public async Task Then_an_existing_learner_record_is_replaced()
        {
            // Arrange
            Request = CreateImportLearnerDetailRequest(LearnerFive);
            
            Request.Ukprn = 444444444;

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            VerifyIlrReplaced(Request, Times.Once);
        }

        [Test]
        public async Task Then_result_is_replace()
        {
            // Arrange
            Request = CreateImportLearnerDetailRequest(LearnerFive);
            
            Request.Ukprn = 444444444;

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            Response.Result.Should().Be("ReplacedLearnerDetail");
        }
    }
}