using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{
    [TestFixture]
    public class When_called_for_existing_learner_having_later_source_with_certificate : ImportLearnerDetailHandlerTestsBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();

            ImportLearnerDetail = CreateImportLearnerDetail(LearnerOne);

            ImportLearnerDetail.Source = "2021";
            ImportLearnerDetail.EpaOrgId = null; // replacement null value
            ImportLearnerDetail.LearnActEndDate = null; // replacement null value
            ImportLearnerDetail.WithdrawReason = null; // replacement null value
            ImportLearnerDetail.Outcome = null; // replacement null value
            ImportLearnerDetail.AchDate = null; // replacement null value
            ImportLearnerDetail.OutGrade = null; // replacement null value    
        }

        [Test]
        public async Task Then_an_existing_learner_record_is_replaced()
        {
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
            VerifyIlrReplaced(ImportLearnerDetail, Times.Once);
        }

        [Test]
        public async Task Then_result_is_replacement()
        {
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
            Response.LearnerDetailResults[0].Outcome.Should().Be("ReplacedLearnerDetail");
        }
    }
}