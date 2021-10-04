using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Threading.Tasks;
using FluentAssertions;
using System;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    [TestFixture]
    public class ApprovalsExtractRepositoryTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private UnitOfWork _unitOfWork;
        private ApprovalsExtractRepository _repository;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var option = new DbContextOptionsBuilder<AssessorDbContext>();
            option.UseSqlServer(_databaseService.WebConfiguration.SqlConnectionString, options => options.EnableRetryOnFailure(3));

            _unitOfWork = new UnitOfWork(new SqlConnection(_databaseService.WebConfiguration.SqlConnectionString));

            _repository = new ApprovalsExtractRepository(_unitOfWork);
        }

        [Test]
        public void When_Empty_Then_NewExtractIsInserted()
        {
            // Arrange

            _databaseService.Execute("TRUNCATE TABLE ApprovalsExtract;");
            var approvalsExtractInput = new List<ApprovalsExtract>()
            {
                new ApprovalsExtract() { ApprenticeshipId = 123 }
            };

            // Act

            _repository.UpsertApprovalsExtract(approvalsExtractInput);

            // Assert

            var approvalsExtractOutput = _databaseService.GetList<ApprovalsExtract>("SELECT * FROM ApprovalsExtract;");
            approvalsExtractOutput.Should().NotBeNullOrEmpty();
            approvalsExtractOutput.Should().HaveCount(1);
        }

        [Test]
        public void When_NotEmpty_Then_ExistingExtractIsUpdated()
        {
            // Arrange

            _databaseService.Execute("TRUNCATE TABLE ApprovalsExtract;");
            _databaseService.Execute("INSERT INTO ApprovalsExtract (ApprenticeshipId, FirstName) VALUES (123, 'TestName');");
            var approvalsExtractInput = new List<ApprovalsExtract>()
            {
                new ApprovalsExtract() { ApprenticeshipId = 123, FirstName = "TestNameUpdated" }
            };

            // Act

            _repository.UpsertApprovalsExtract(approvalsExtractInput);

            // Assert

            var approvalsExtractOutput = _databaseService.GetList<ApprovalsExtract>("SELECT * FROM ApprovalsExtract;");
            approvalsExtractOutput.Should().NotBeNullOrEmpty();
            approvalsExtractOutput.Should().HaveCount(1);
            approvalsExtractOutput.ElementAt(0).FirstName.Should().Be("TestNameUpdated");
        }

        [Test]
        public void When_NotEmpty_Then_NewIsInsertedAndExistingIsUpdated()
        {
            // Arrange

            _databaseService.Execute("TRUNCATE TABLE ApprovalsExtract;");
            _databaseService.Execute("INSERT INTO ApprovalsExtract (ApprenticeshipId, FirstName) VALUES (123, 'TestName');");
            var approvalsExtractInput = new List<ApprovalsExtract>()
            {
                new ApprovalsExtract() { ApprenticeshipId = 123, FirstName = "TestNameUpdated" },
                new ApprovalsExtract() { ApprenticeshipId = 456, FirstName = "SecondTestName" }
            };

            // Act

            _repository.UpsertApprovalsExtract(approvalsExtractInput);

            // Assert

            var approvalsExtractOutput = _databaseService.GetList<ApprovalsExtract>("SELECT * FROM ApprovalsExtract ORDER BY ApprenticeshipId;");
            approvalsExtractOutput.Should().NotBeNullOrEmpty();
            approvalsExtractOutput.Should().HaveCount(2);
            approvalsExtractOutput.ElementAt(0).FirstName.Should().Be("TestNameUpdated");
        }

        [Test]
        public void When_ExtractHasAllFieldsPopulated_Then_AllFieldsAreInserted()
        {
            // Arrange

            _databaseService.Execute("TRUNCATE TABLE ApprovalsExtract;");
            var approvalExtractInput = new ApprovalsExtract()
            {
                ApprenticeshipId = 123,
                FirstName = "Testfirstname",
                LastName = "Testlastname",
                ULN = "8028125094",
                TrainingCode = 196,
                TrainingCourseVersion = "1.1",
                TrainingCourseVersionConfirmed = true,
                TrainingCourseOption = "French",
                StandardUId = "ST0002_1.1",
                StartDate = new DateTime(2021, 03, 01),
                EndDate = new DateTime(2022, 06, 01),
                CreatedOn = new DateTime(2021, 10, 03),
                UpdatedOn = new DateTime(2021, 10, 04),
                StopDate = new DateTime(2022, 08, 01),
                CompletionDate = new DateTime(2022, 09, 01),
                PauseDate = new DateTime(2021, 12, 01),
                UKPRN = 10006600,
                LearnRefNumber = "RF5764700785",
                PaymentStatus = 1
            };
            var approvalsExtractInput = new List<ApprovalsExtract>()
            {
                approvalExtractInput
            };

            // Act

            _repository.UpsertApprovalsExtract(approvalsExtractInput);

            // Assert

            var approvalsExtractOutput = _databaseService.GetList<ApprovalsExtract>("SELECT * FROM ApprovalsExtract;");
            approvalsExtractOutput.Should().NotBeNullOrEmpty();
            approvalsExtractOutput.Should().HaveCount(1);
            approvalsExtractOutput.ElementAt(0).Should().BeEquivalentTo(approvalExtractInput);
        }
    }
}
