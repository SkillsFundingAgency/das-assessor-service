using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{
    [TestFixture]
    public class When_called_exist_later_source_without_certificate : ImportLearnerDetailHandlerTestsBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();

            // Arrange
            ImportLearnerDetail = CreateImportLearnerDetail(LearnerWithoutCertificate);
            ImportLearnerDetail.Source = "2021"; // a later source to which should trigger a replacement, the replacement values are null
            ImportLearnerDetail.EpaOrgId = null;
            ImportLearnerDetail.LearnActEndDate = null;
            ImportLearnerDetail.WithdrawReason = null;
            ImportLearnerDetail.Outcome = null;
            ImportLearnerDetail.AchDate = null;
            ImportLearnerDetail.OutGrade = null;

            Request = new ImportLearnerDetailRequest
            {
                ImportLearnerDetails = new List<ImportLearnerDetail>
                {
                    ImportLearnerDetail
                }
            };
        }

        [Test]
        public async Task Then_an_existing_learner_record_is_replaced()
        {
            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            VerifyIlrReplaced(Request.ImportLearnerDetails[0], Times.Once);
        }

        [Test]
        public async Task Then_result_is_replacement()
        {
            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            Response.LearnerDetailResults.Count.Should().Be(1);
            Response.LearnerDetailResults[0].Outcome.Should().Be("ReplacedLearnerDetail");
        }
    }
}