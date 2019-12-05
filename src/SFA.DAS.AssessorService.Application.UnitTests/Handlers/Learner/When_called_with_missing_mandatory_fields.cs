using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{
    [TestFixture]
    public class When_called_with_missing_mandatory_fields : ImportLearnerDetailHandlerTestsBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();
        }

        [TestCase(null, 111111, 111111, 11, 11, "given", "family", "01-01-2000", "01-01-2000", 1, "1", "1")]
        [TestCase("1920", null, 111111, 11, 11, "given", "family", "01-01-2000", "01-01-2000", 1, "1", "1")]
        [TestCase("1920", 111111, null, 11, 11, "given", "family", "01-01-2000", "01-01-2000", 1, "1", "1")]
        [TestCase("1920", 111111, 11111, null, 11, "given", "family", "01-01-2000", "01-01-2000", 1, "1", "1")]
        [TestCase("1920", 111111, 11111, 1111, null, "given", "family", "01-01-2000", "01-01-2000", 1, "1", "1")]
        [TestCase("1920", 111111, 11111, 1111, 11, null, "family", "01-01-2000", "01-01-2000", 1, "1", "1")]
        [TestCase("1920", 111111, 11111, 1111, 11, "given", null, "01-01-2000", "01-01-2000", 1, "1", "1")]
        [TestCase("1920", 111111, 11111, 1111, 11, "given", "family", null, "01-01-2000", 1, "1", "1")]
        [TestCase("1920", 111111, 11111, 1111, 11, "given", "family", "01-01-2000", null, 1, "1", "1")]
        [TestCase("1920", 111111, 11111, 1111, 11, "given", "family", "01-01-2000", "01-01-2000", null, "1", "1")]
        [TestCase("1920", 111111, 11111, 1111, 11, "given", "family", "01-01-2000", "01-01-2000", 1, null, "1")]
        [TestCase("1920", 111111, 11111, 1111, 11, "given", "family", "01-01-2000", "01-01-2000", 1, "1", null)]
        [TestCase("1920", 111111, 11111, 1111, 11, null, "family", null, "01-01-2000", 1, "1", "1")]
        public async Task Then_learner_records_are_unchanged(string source, long? ukprn, long? uln, int? stdCode,
            int? fundingModel, string givenNames, string familyName, string learnStartDate, string plannedEndDate,
            int? completionStatus, string learnRefNumber, string delLocPostCode)
        {
            // Arrange
            Request = CreateImportLearnerDetailRequest(source, ukprn, uln, stdCode, fundingModel, givenNames, familyName,
                (learnStartDate == null ? (DateTime?)null : DateTime.Parse(learnStartDate)),
                (plannedEndDate == null ? (DateTime?)null : DateTime.Parse(plannedEndDate)),
                completionStatus, learnRefNumber, delLocPostCode);

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            IlrRepository.Verify(r => r.Create(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int?>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int?>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(), It.IsAny<string>()), Times.Never);

            // Assert
            IlrRepository.Verify(r => r.Update(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int?>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int?>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(), It.IsAny<string>()), Times.Never);
        }

        [TestCase(null, 111111, 111111, 11, 11, "given", "family", "01-01-2000", "01-01-2000", 1, "1", "1", "Source")]
        [TestCase("1920", null, 111111, 11, 11, "given", "family", "01-01-2000", "01-01-2000", 1, "1", "1", "Ukprn")]
        [TestCase("1920", 111111, null, 11, 11, "given", "family", "01-01-2000", "01-01-2000", 1, "1", "1", "Uln")]
        [TestCase("1920", 111111, 11111, null, 11, "given", "family", "01-01-2000", "01-01-2000", 1, "1", "1", "StdCode")]
        [TestCase("1920", 111111, 11111, 1111, null, "given", "family", "01-01-2000", "01-01-2000", 1, "1", "1", "FundingModel")]
        [TestCase("1920", 111111, 11111, 1111, 11, null, "family", "01-01-2000", "01-01-2000", 1, "1", "1", "GivenNames")]
        [TestCase("1920", 111111, 11111, 1111, 11, "given", null, "01-01-2000", "01-01-2000", 1, "1", "1", "FamilyName")]
        [TestCase("1920", 111111, 11111, 1111, 11, "given", "family", null, "01-01-2000", 1, "1", "1", "LearnStartDate")]
        [TestCase("1920", 111111, 11111, 1111, 11, "given", "family", "01-01-2000", null, 1, "1", "1", "PlannedEndDate")]
        [TestCase("1920", 111111, 11111, 1111, 11, "given", "family", "01-01-2000", "01-01-2000", null, "1", "1", "CompletionStatus")]
        [TestCase("1920", 111111, 11111, 1111, 11, "given", "family", "01-01-2000", "01-01-2000", 1, null, "1", "LearnRefNumber")]
        [TestCase("1920", 111111, 11111, 1111, 11, "given", "family", "01-01-2000", "01-01-2000", 1, "1", null, "DelLocPostCode")]
        [TestCase("1920", 111111, 11111, 1111, 11, null, "family", null, "01-01-2000", 1, "1", "1", "GivenNames,LearnStartDate")]
        public async Task Then_result_is_error_missing_mandatory_field(string source, long? ukprn, long? uln, int? stdCode,
            int? fundingModel, string givenNames, string familyName, string learnStartDate, string plannedEndDate,
            int? completionStatus, string learnRefNumber, string delLocPostCode, string missingFieldNames)
        {
            // Arrange
            Request = CreateImportLearnerDetailRequest(source, ukprn, uln, stdCode, fundingModel, givenNames, familyName,
                (learnStartDate == null ? (DateTime?)null : DateTime.Parse(learnStartDate)), 
                (plannedEndDate == null ? (DateTime?)null : DateTime.Parse(plannedEndDate)), 
                completionStatus, learnRefNumber, delLocPostCode);

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            Response.Result.Should().Be("ErrorMissingMandatoryField");
            foreach (string missingFieldName in missingFieldNames.Split(','))
            {
                Response.Errors.Should().Contain(p => p.Contains(missingFieldName));
            }
        }
    }
}