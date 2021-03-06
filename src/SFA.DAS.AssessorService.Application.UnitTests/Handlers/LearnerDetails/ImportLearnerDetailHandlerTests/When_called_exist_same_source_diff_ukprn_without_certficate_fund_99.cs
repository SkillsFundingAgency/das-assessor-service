﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{
    [TestFixture]
    public class When_called_for_existing_learner_having_same_source_and_different_ukprn_without_certficate_and_funding_model_99 : ImportLearnerDetailHandlerTestsBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();
        }

        [Test]
        public async Task Then_learner_records_are_not_created()
        {
            // Arrange
            ImportLearnerDetail = CreateImportLearnerDetail(LearnerTwo.Source, 444444444, LearnerTwo.Uln, LearnerTwo.StdCode,
                99, LearnerTwo.GivenNames, LearnerTwo.FamilyName, LearnerTwo.EpaOrgId,
                LearnerTwo.LearnStartDate, LearnerTwo.PlannedEndDate,
                LearnerTwo.CompletionStatus, LearnerTwo.LearnRefNumber, LearnerTwo.DelLocPostCode,
                LearnerTwo.LearnActEndDate, LearnerTwo.WithdrawReason, LearnerTwo.Outcome, LearnerTwo.AchDate, LearnerTwo.OutGrade);

            ImportLearnerDetailRequest request = new ImportLearnerDetailRequest
            {
                ImportLearnerDetails = new List<ImportLearnerDetail>
                {
                    ImportLearnerDetail
                }
            };

            // Act
            Response = await Sut.Handle(request, new CancellationToken());

            // Assert
            IlrRepository.Verify(r => r.Create(It.IsAny<Ilr>()), Times.Never);
        }

        [Test]
        public async Task Then_learner_records_are_not_updated()
        {
            // Arrange
            ImportLearnerDetail = CreateImportLearnerDetail(LearnerTwo.Source, 444444444, LearnerTwo.Uln, LearnerTwo.StdCode,
                99, LearnerTwo.GivenNames, LearnerTwo.FamilyName, LearnerTwo.EpaOrgId,
                LearnerTwo.LearnStartDate, LearnerTwo.PlannedEndDate,
                LearnerTwo.CompletionStatus, LearnerTwo.LearnRefNumber, LearnerTwo.DelLocPostCode,
                LearnerTwo.LearnActEndDate, LearnerTwo.WithdrawReason, LearnerTwo.Outcome, LearnerTwo.AchDate, LearnerTwo.OutGrade);

            ImportLearnerDetailRequest request = new ImportLearnerDetailRequest
            {
                ImportLearnerDetails = new List<ImportLearnerDetail>
                {
                    ImportLearnerDetail
                }
            };

            // Act
            Response = await Sut.Handle(request, new CancellationToken());

            // Assert
            IlrRepository.Verify(r => r.Update(It.IsAny<Ilr>()), Times.Never);
        }


        [Test]
        public async Task Then_result_is_ignore_ukprn_changed_for_funding_model_99()
        {
            // Arrange
            ImportLearnerDetail = CreateImportLearnerDetail(LearnerTwo.Source, 444444444, LearnerTwo.Uln, LearnerTwo.StdCode,
                99, LearnerTwo.GivenNames, LearnerTwo.FamilyName, LearnerTwo.EpaOrgId,
                LearnerTwo.LearnStartDate, LearnerTwo.PlannedEndDate,
                LearnerTwo.CompletionStatus, LearnerTwo.LearnRefNumber, LearnerTwo.DelLocPostCode,
                LearnerTwo.LearnActEndDate, LearnerTwo.WithdrawReason, LearnerTwo.Outcome, LearnerTwo.AchDate, LearnerTwo.OutGrade);

            ImportLearnerDetailRequest request = new ImportLearnerDetailRequest
            {
                ImportLearnerDetails = new List<ImportLearnerDetail>
                {
                    ImportLearnerDetail
                }
            };

            // Act
            Response = await Sut.Handle(request, new CancellationToken());

            // Assert
            Response.LearnerDetailResults.Count.Should().Be(1);
            Response.LearnerDetailResults[0].Outcome.Should().Be("IgnoreFundingModelChangedTo99WhenPrevioulsyNot99");
        }
    }
}