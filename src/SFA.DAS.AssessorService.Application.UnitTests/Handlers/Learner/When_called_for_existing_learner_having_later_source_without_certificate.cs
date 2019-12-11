using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{
    [TestFixture]
    public class When_called_for_existing_learner_having_later_source_without_certificate : ImportLearnerDetailHandlerTestsBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();

            Request = CreateImportLearnerDetailRequest(LearnerTwo);

            Request.Source = "2021";
            Request.EpaOrgId = null; // replacement null value
            Request.LearnActEndDate = null; // replacement null value
            Request.WithdrawReason = null; // replacement null value
            Request.Outcome = null; // replacement null value
            Request.AchDate = null; // replacement null value
            Request.OutGrade = null; // replacement null value    
        }

        [Test]
        public async Task Then_an_existing_learner_record_is_replaced()
        {
            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            VerifyIlrReplaced(Request, Times.Once);
        }

        [Test]
        public async Task Then_result_is_replacement()
        {
            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            Response.Result.Should().Be("ReplacedLearnerDetail");
        }
    }
}