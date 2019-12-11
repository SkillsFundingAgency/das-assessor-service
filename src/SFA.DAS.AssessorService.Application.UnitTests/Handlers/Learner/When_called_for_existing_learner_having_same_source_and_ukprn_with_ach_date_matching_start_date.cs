using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{
    [TestFixture]
    public class When_called_for_existing_learner_having_same_source_and_ukprn_with_ach_date_matching_start_date : ImportLearnerDetailHandlerTestsBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();

            Request = CreateImportLearnerDetailRequest(LearnerOne); // Source and Ukprn unchanged
            Request.LearnActEndDate = Request.LearnStartDate; // LearnActEndDate is the same as LearnStartDate
        }

        [Test]
        public async Task Then_learner_records_are_not_created()
        {
            // Act
            Response = await Sut.Handle(Request, new CancellationToken());
            
            // Assert
            IlrRepository.Verify(r => r.Create(It.IsAny<Ilr>()), Times.Never);
        }

        [Test]
        public async Task Then_learner_records_are_not_updated()
        {
            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            IlrRepository.Verify(r => r.Update(It.IsAny<Ilr>()), Times.Never);
        }

        [Test]
        public async Task Then_result_is_ignore_dummy_learn_act_end_date()
        {
            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            Response.Result.Should().Be("IgnoreLearnActEndDateSameAsLearnStartDate");
        }
    }
}