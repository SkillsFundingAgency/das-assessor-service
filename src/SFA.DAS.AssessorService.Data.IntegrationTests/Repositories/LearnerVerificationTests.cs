using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories
{
    public class LearnerVerificationTests : TestBase
    {
        [TestCase(123456789L, 123)]
        [TestCase(null, 123)]
        [TestCase(123456789L, null)]
        [TestCase(null, null)]
        public async Task GetByUlnAndStdCodeAsync_WhenOtherLearnersExist_ThenReturnsOnlyTheRequestedLearner(long? uln, int? stdCode)
        {
            var learner = CreateLearner(uln, stdCode);
            var differentUln = CreateLearner(987654321, stdCode);
            var differentStandard = CreateLearner(uln, 456);

            using (var _sut = new LearnerVerificationTestsFixture()
                .WithLearner(differentUln)
                .WithLearner(differentStandard)
                .WithLearner(learner))
            {
                var actual = await LearnerHandler.GetByUlnAndStdCodeAsync(uln, stdCode);

                actual.Should().BeEquivalentTo(learner);
                (await LearnerHandler.GetByUlnAndStdCodeAsync(111111111, 999)).Should().BeNull();
            }
        }

        [TestCase(nameof(LearnerModel.DateOfBirth))]
        [TestCase(nameof(LearnerModel.IsTransfer))]
        public void VerifyLearnerExists_WhenAPropertyDiffers_ThenFailureNamesTheProperty(string propertyName)
        {
            var learner = CreateLearner();
            var expected = CreateLearner();
            expected.Id = null;

            if (propertyName == nameof(LearnerModel.DateOfBirth))
            {
                expected.DateOfBirth = learner.DateOfBirth.Value.AddYears(1);
            }
            else
            {
                expected.IsTransfer = 1;
            }

            using (var _sut = new LearnerVerificationTestsFixture().WithLearner(learner))
            {
                var exception = Assert.ThrowsAsync<AssertionException>(() => _sut.VerifyLearnerExists(expected));

                exception.Message.Should().Contain(propertyName);
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task VerifyLearnerExists_WhenIdIsUnspecifiedOrMatches_ThenReturnsTheFixture(bool specifyId)
        {
            var learner = CreateLearner();
            var expected = CreateLearner();
            expected.Id = specifyId ? learner.Id : null;

            using (var _sut = new LearnerVerificationTestsFixture().WithLearner(learner))
            {
                var result = await _sut.VerifyLearnerExists(expected);

                result.Should().BeSameAs(_sut);
            }
        }

        [Test]
        public void VerifyLearnerExists_WhenAnExplicitIdDiffers_ThenFailureNamesId()
        {
            var learner = CreateLearner();
            var expected = CreateLearner();

            using (var _sut = new LearnerVerificationTestsFixture().WithLearner(learner))
            {
                var exception = Assert.ThrowsAsync<AssertionException>(() => _sut.VerifyLearnerExists(expected));

                exception.Message.Should().Contain(nameof(LearnerModel.Id));
            }
        }

        [TestCase(3, 800, true)]
        [TestCase(3, 1000, false)]
        public async Task VerifyLearnerExists_WhenTimestampsHaveFractions_ThenComparesWholeSeconds(
            int actualMilliseconds, int expectedMilliseconds, bool shouldMatch)
        {
            var timestamp = new DateTime(2026, 10, 22, 12, 15, 30);
            var learner = CreateLearner();
            learner.LatestIlrs = timestamp.AddMilliseconds(actualMilliseconds);
            var expected = CreateLearner();
            expected.Id = null;
            expected.LatestIlrs = timestamp.AddMilliseconds(expectedMilliseconds);

            using (var _sut = new LearnerVerificationTestsFixture().WithLearner(learner))
            {
                if (shouldMatch)
                {
                    await _sut.VerifyLearnerExists(expected);
                }
                else
                {
                    var exception = Assert.ThrowsAsync<AssertionException>(() => _sut.VerifyLearnerExists(expected));
                    exception.Message.Should().Contain(nameof(LearnerModel.LatestIlrs));
                }
            }
        }

        [Test]
        public void VerifyLearnerExists_WhenExpectedTimeIsMidnight_ThenRequiresAnExactMatch()
        {
            var midnight = new DateTime(2026, 10, 22);
            var learner = CreateLearner();
            learner.LatestIlrs = midnight.AddMilliseconds(3);
            var expected = CreateLearner();
            expected.Id = null;
            expected.LatestIlrs = midnight;

            using (var _sut = new LearnerVerificationTestsFixture().WithLearner(learner))
            {
                var exception = Assert.ThrowsAsync<AssertionException>(() => _sut.VerifyLearnerExists(expected));

                exception.Message.Should().Contain(nameof(LearnerModel.LatestIlrs));
            }
        }

        private static LearnerModel CreateLearner(long? uln = 123456789, int? stdCode = 123)
        {
            return new LearnerModel
            {
                Id = Guid.NewGuid(),
                Uln = uln,
                StdCode = stdCode,
                DateOfBirth = new DateTime(2000, 3, 7)
            };
        }

        private class LearnerVerificationTestsFixture : FixtureBase<LearnerVerificationTestsFixture>, IDisposable
        {
            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
