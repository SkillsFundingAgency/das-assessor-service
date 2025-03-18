using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.UnitTests.Mapping
{
    [TestFixture]
    public class FrameworkLearnerMappingTests : MapperBase
    {
        [Test, RecursiveMoqAutoData]
        public void ShouldMapFrameworkLearnerToFrameworkSearchResult(FrameworkLearner frameworkLearner)
        {
            // Act
            var response = Mapper.Map<FrameworkLearnerSearchResponse>(frameworkLearner);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(frameworkLearner.Id);
            response.FrameworkName.Should().Be(frameworkLearner.FrameworkName);
            response.ApprenticeshipLevelName.Should().Be(frameworkLearner.ApprenticeshipLevelName);
            response.CertificationYear.Should().Be(frameworkLearner.CertificationYear);
        }

        [Test, RecursiveMoqAutoData]
        public void ShouldMapListOfFrameworkLearnerToListOfFrameworkSearchResult(List<FrameworkLearner> learners)
        {
            // Act
            var responses = Mapper.Map<List<FrameworkLearnerSearchResponse>>(learners);

            // Assert
            responses.Should().NotBeNull();
            responses.Count.Should().Be(learners.Count);

            for (int i = 0; i < responses.Count; i++)
            {
                ShouldMapFrameworkLearnerToFrameworkSearchResult(learners[i]);
            }
        }
    }
}
