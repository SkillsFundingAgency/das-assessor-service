﻿using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{
    [TestFixture]
    public class When_called_dummy_uln : ImportLearnerDetailHandlerTestsBase
    {
        public void Arrange(long uln)
        {
            BaseArrange();

            // Arrange
            ImportLearnerDetail = CreateImportLearnerDetail(LearnerWithCertificate);
            ImportLearnerDetail.Uln = uln;

            // Arrange
            Request = new ImportLearnerDetailRequest
            {
                ImportLearnerDetails = new List<ImportLearnerDetail>
                {
                    ImportLearnerDetail
                }
            };
        }

        [TestCaseSource(nameof(TestSource))]
        public async Task Then_no_learner_records_are_created(long uln)
        {
            // Arrange
            Arrange(uln);

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            IlrRepository.Verify(r => r.Create(It.IsAny<Ilr>()), Times.Never);
        }

        [TestCaseSource(nameof(TestSource))]
        public async Task Then_no_learner_records_are_updated(long uln)
        {
            // Arrange
            Arrange(uln);

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            IlrRepository.Verify(r => r.Update(It.IsAny<Ilr>()), Times.Never);
        }

        [TestCaseSource(nameof(TestSource))]
        public async Task Then_result_is_ignore_dummy_uln(long uln)
        {
            // Arrange
            Arrange(uln);

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            Response.LearnerDetailResults.Count.Should().Be(1);
            Response.LearnerDetailResults[0].Outcome.Should().Be("IgnoreUlnDummyValue");
        }

        static IEnumerable<object[]> TestSource()
        {
            return new[]
            {
                new object[] { 9999999999 },
                new object[] { 1000000000 }
            };
        }
    }
}