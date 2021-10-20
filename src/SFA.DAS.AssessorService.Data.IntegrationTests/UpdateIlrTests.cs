using AutoFixture;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    public class UpdateIlrTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();

        private SqlConnection _databaseConnection;
        private UnitOfWork _unitOfWork;
        private IlrRepository _ilrRepository;

        private Ilr _originalIlr;

        private Fixture _fixture;

        [OneTimeSetUp]
        public void SetupUpdateIlrTests()
        {
            _fixture = new Fixture();

            _databaseConnection = new SqlConnection(_databaseService.WebConfiguration.SqlConnectionString);
            _unitOfWork = new UnitOfWork(_databaseConnection);

            _originalIlr = CreateOriginalIlr();

            _ilrRepository = new IlrRepository(_unitOfWork);

            IlrHandler.InsertRecord(CreateIlrModel(_originalIlr));
        }

        [Test]
        public async Task When_UpdateIlr_And_NoChangesMade_Then_DoNotUpdate()
        {
            // Act
            await _ilrRepository.Update(_originalIlr);

            // Assert
            var result = await _ilrRepository.Get(_originalIlr.Uln, _originalIlr.StdCode);

            AssertEqual(_originalIlr, result);
            Assert.That(_originalIlr.UpdatedAt, Is.EqualTo(result.UpdatedAt).Within(10).Milliseconds);
        }

        [Test]
        public async Task When_UpdateIlr_And_ChangesAreMade_Then_UpdateChanges_And_SetUpdatedAtValueToCurrentTime()
        {
            // Arrange
            var ilrWithUpdates = CloneIlrWithChanges(_originalIlr);

            // Act
            await _ilrRepository.Update(ilrWithUpdates);

            // Assert
            var result = await _ilrRepository.Get(_originalIlr.Uln, _originalIlr.StdCode);

            AssertEqual(ilrWithUpdates, result);
            Assert.That(DateTime.UtcNow, Is.EqualTo(result.UpdatedAt).Within(1).Seconds);
        }

        private void AssertEqual(Ilr expected, Ilr result)
        {
            Assert.AreEqual(expected.Id, result.Id);
            Assert.AreEqual(expected.Source, result.Source);
            Assert.AreEqual(expected.UkPrn, result.UkPrn);
            Assert.AreEqual(expected.StdCode, result.StdCode);
            Assert.AreEqual(expected.FundingModel, result.FundingModel);
            Assert.AreEqual(expected.GivenNames, result.GivenNames);
            Assert.AreEqual(expected.FamilyName, result.FamilyName);
            Assert.AreEqual(expected.EpaOrgId, result.EpaOrgId);
            Assert.AreEqual(expected.CompletionStatus, result.CompletionStatus);
            Assert.AreEqual(expected.LearnRefNumber, result.LearnRefNumber);
            Assert.AreEqual(expected.DelLocPostCode, result.DelLocPostCode); 
            Assert.AreEqual(expected.WithdrawReason, result.WithdrawReason);
            Assert.AreEqual(expected.Outcome, result.Outcome);
            Assert.AreEqual(expected.OutGrade, result.OutGrade);

            Assert.That(expected.LearnStartDate, Is.EqualTo(result.LearnStartDate).Within(10).Milliseconds);
            Assert.That(expected.PlannedEndDate, Is.EqualTo(result.PlannedEndDate).Within(10).Milliseconds);
            Assert.That(expected.LearnActEndDate, Is.EqualTo(result.LearnActEndDate).Within(10).Milliseconds);
            Assert.That(expected.CreatedAt, Is.EqualTo(result.CreatedAt).Within(10).Milliseconds);
        }


        private Ilr CreateOriginalIlr()
        {
            var ilr = _fixture.Build<Ilr>()
                .Without(x => x.UpdatedAt)
                .With(x => x.CreatedAt, DateTime.Now.AddDays(-1))
                .With(x => x.Source, _fixture.Create<int>().ToString())
                .With(x => x.LearnRefNumber, _fixture.Create<int>().ToString())
                .Create();

            ilr.UpdatedAt = ilr.CreatedAt;

            return ilr;
        }

        private Ilr CloneIlrWithChanges(Ilr ilr)
        {
            return new Ilr
            {
                Id = ilr.Id,
                Source = ilr.Source,
                UkPrn = ilr.UkPrn,
                Uln = ilr.Uln,
                StdCode = ilr.StdCode,
                FundingModel = ilr.FundingModel,
                GivenNames = _fixture.Create<string>(),
                FamilyName = _fixture.Create<string>(),
                EpaOrgId = ilr.EpaOrgId,
                LearnStartDate = ilr.LearnStartDate,
                PlannedEndDate = ilr.PlannedEndDate,
                CompletionStatus = ilr.CompletionStatus,
                LearnRefNumber = ilr.LearnRefNumber,
                DelLocPostCode = ilr.DelLocPostCode,
                LearnActEndDate = ilr.LearnActEndDate,
                Outcome = ilr.Outcome,
                WithdrawReason = ilr.WithdrawReason,
                AchDate = ilr.AchDate,
                OutGrade = ilr.OutGrade,
                CreatedAt = ilr.CreatedAt
            };
        }

        private IlrModel CreateIlrModel(Ilr ilr)
        {
            return new IlrModel
            {
                Id = ilr.Id,
                Source = ilr.Source,
                UkPrn = ilr.UkPrn,
                Uln = ilr.Uln,
                StdCode = ilr.StdCode,
                FundingModel = ilr.FundingModel,
                GivenNames = ilr.GivenNames,
                FamilyName = ilr.FamilyName,
                EpaOrgId = ilr.EpaOrgId,
                LearnStartDate = ilr.LearnStartDate,
                PlannedEndDate = ilr.PlannedEndDate,
                CompletionStatus = ilr.CompletionStatus,
                LearnRefNumber = ilr.LearnRefNumber,
                DelLocPostCode = ilr.DelLocPostCode,
                LearnActEndDate = ilr.LearnActEndDate,
                Outcome = ilr.Outcome,
                WithdrawReason = ilr.WithdrawReason,
                AchDate = ilr.AchDate,
                OutGrade = ilr.OutGrade,
                CreatedAt = ilr.CreatedAt,
                UpdatedAt = ilr.UpdatedAt
            };
        }
    }
}
