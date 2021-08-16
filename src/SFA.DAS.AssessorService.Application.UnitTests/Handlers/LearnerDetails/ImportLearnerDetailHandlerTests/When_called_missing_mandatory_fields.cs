using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{
    [TestFixture]
    public class When_called_with_missing_mandatory_fields : ImportLearnerDetailHandlerTestsBase
    {
        public void Arrange(string source, int? ukprn, long? uln, int? stdCode,
            int? fundingModel, string givenNames, string familyName, string learnStartDate, string plannedEndDate,
            int? completionStatus, string learnRefNumber, string delLocPostCode)
        {
            BaseArrange();

            // Arrange
            ImportLearnerDetail = CreateImportLearnerDetail(source, ukprn, uln, stdCode, fundingModel, givenNames, familyName,
                (learnStartDate == null ? (DateTime?)null : DateTime.Parse(learnStartDate, CultureInfo.InvariantCulture)),
                (plannedEndDate == null ? (DateTime?)null : DateTime.Parse(plannedEndDate, CultureInfo.InvariantCulture)),
                completionStatus, learnRefNumber, delLocPostCode);

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
        public async Task Then_learner_records_are_not_created(string source, int? ukprn, long? uln, int? stdCode,
            int? fundingModel, string givenNames, string familyName, string learnStartDate, string plannedEndDate,
            int? completionStatus, string learnRefNumber, string delLocPostCode, string missingFieldNames)
        {
            // Arrange
            Arrange(source, ukprn, uln, stdCode, fundingModel, givenNames, familyName, learnStartDate, plannedEndDate,
                completionStatus, learnRefNumber, delLocPostCode);

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            IlrRepository.Verify(r => r.Create(It.IsAny<Ilr>()), Times.Never);
        }

        [TestCaseSource(nameof(TestSource))]
        public async Task Then_learner_records_are_not_updated(string source, int? ukprn, long? uln, int? stdCode,
            int? fundingModel, string givenNames, string familyName, string learnStartDate, string plannedEndDate,
            int? completionStatus, string learnRefNumber, string delLocPostCode, string missingFieldNames)
        {
            // Arrange
            Arrange(source, ukprn, uln, stdCode, fundingModel, givenNames, familyName, learnStartDate, plannedEndDate,
                completionStatus, learnRefNumber, delLocPostCode);

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            IlrRepository.Verify(r => r.Update(It.IsAny<Ilr>()), Times.Never);
        }

        [TestCaseSource(nameof(TestSource))]
        public async Task Then_result_is_error_missing_mandatory_field(string source, int? ukprn, long? uln, int? stdCode,
            int? fundingModel, string givenNames, string familyName, string learnStartDate, string plannedEndDate,
            int? completionStatus, string learnRefNumber, string delLocPostCode, string missingFieldNames)
        {
            // Arrange
            Arrange(source, ukprn, uln, stdCode, fundingModel, givenNames, familyName, learnStartDate, plannedEndDate,
                completionStatus, learnRefNumber, delLocPostCode);

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            Response.LearnerDetailResults.Count.Should().Be(1);
            Response.LearnerDetailResults[0].Outcome.Should().Be("ErrorMissingMandatoryField");
            foreach (string missingFieldName in missingFieldNames.Split(','))
            {
                Response.LearnerDetailResults[0].Errors.Should().Contain(p => p.Contains(missingFieldName));
            }
        }

        static IEnumerable<object[]> TestSource()
        {
            return new[]
            {
                new object[] { null, 111111, (long?)111111, 11, 11, "given", "family", "01/01/2000", "01/01/2000", 1, "1", "1", "Source" },
                new object[] { "1920", null, (long?)111111, 11, 11, "given", "family", "01/01/2000", "01/01/2000", 1, "1", "1", "Ukprn" },
                new object[] { "1920", 111111, null, 11, 11, "given", "family", "01/01/2000", "01/01/2000", 1, "1", "1", "Uln" },
                new object[] { "1920", 111111, (long?)11111, null, 11, "given", "family", "01/01/2000", "01/01/2000", 1, "1", "1", "StdCode" },
                new object[] { "1920", 111111, (long?)11111, 1111, null, "given", "family", "01/01/2000", "01/01/2000", 1, "1", "1", "FundingModel" },
                new object[] { "1920", 111111, (long?)11111, 1111, 11, null, "family", "01/01/2000", "01/01/2000", 1, "1", "1", "GivenNames" },
                new object[] { "1920", 111111, (long?)11111, 1111, 11, "given", null, "01/01/2000", "01/01/2000", 1, "1", "1", "FamilyName" },
                new object[] { "1920", 111111, (long?)11111, 1111, 11, "given", "family", null, "01/01/2000", 1, "1", "1", "LearnStartDate" },
                new object[] { "1920", 111111, (long?)11111, 1111, 11, "given", "family", "01/01/2000", null, 1, "1", "1", "PlannedEndDate" },
                new object[] { "1920", 111111, (long?)11111, 1111, 11, "given", "family", "01/01/2000", "01/01/2000", null, "1", "1", "CompletionStatus" },
                new object[] { "1920", 111111, (long?)11111, 1111, 11, "given", "family", "01/01/2000", "01/01/2000", 1, null, "1", "LearnRefNumber" },
                new object[] { "1920", 111111, (long?)11111, 1111, 11, "given", "family", "01/01/2000", "01/01/2000", 1, "1", null, "DelLocPostCode" },
                new object[] { "1920", 111111, (long?)11111, 1111, 11, null, "family", null, "01/01/2000", 1, "1", "1", "GivenNames,LearnStartDate" }
            };
        }
    }
}