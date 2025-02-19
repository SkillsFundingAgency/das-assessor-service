using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Mapping
{
    [TestFixture]
    public class FrameworkLearnerMappingTests : MapperBase
    {
        [Test, RecursiveMoqAutoData]
        public void ShouldMapFrameworkLearnerToFrameworkSearchResult(FrameworkLearner frameworkLearner)
        {
            // Act
            var response = Mapper.Map<FrameworkSearchResult>(frameworkLearner);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(frameworkLearner.Id);
            response.FrameworkName.Should().Be(frameworkLearner.FrameworkName);
            response.ApprenticeshipLevel.Should().Be(frameworkLearner.ApprenticeshipLevel);
            response.CertificationYear.Should().Be(frameworkLearner.CertificationYear);
        }

        [Test, RecursiveMoqAutoData]
        public void ShouldMapListOfFrameworkLearnerToListOfFrameworkSearchResult(List<FrameworkLearner> learners)
        {
            // Act
            var responses = Mapper.Map<List<FrameworkSearchResult>>(learners);

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
