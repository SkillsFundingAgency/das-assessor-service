﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    [TestFixture]
    public class ApprovalsExtractRepositoryTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private IUnitOfWork _unitOfWork;
        private ApprovalsExtractRepository _repository;

        [OneTimeSetUp]
        public void SetupApprovalsExtractTests()
        {
            _unitOfWork = new UnitOfWork(new SqlConnection(_databaseService.SqlConnectionStringTest));

            _repository = new ApprovalsExtractRepository(_unitOfWork, new Mock<IRoatpApiClient>().Object, new Mock<ILogger<ApprovalsExtractRepository>>().Object);
        }

        [Test]
        public async Task When_Empty_Then_NewExtractIsInserted()
        {
            // Arrange
            _databaseService.Execute("TRUNCATE TABLE ApprovalsExtract_Staging;");
            _databaseService.Execute("TRUNCATE TABLE ApprovalsExtract;");
            var approvalsExtractInput = new List<ApprovalsExtract>()
            {
                new ApprovalsExtract() { ApprenticeshipId = 123, ULN = "123456789" }
            };

            await _repository.UpsertApprovalsExtractToStaging(approvalsExtractInput);
            // Act

            await _repository.PopulateApprovalsExtract();

            // Assert

            var approvalsExtractOutput = _databaseService.GetList<ApprovalsExtract>("SELECT * FROM ApprovalsExtract;");
            approvalsExtractOutput.Should().NotBeNullOrEmpty();
            approvalsExtractOutput.Should().HaveCount(1);
        }

        [Test]
        public async Task When_NotEmpty_Then_ExistingExtractIsUpdated()
        {
            // Arrange
            _databaseService.Execute("TRUNCATE TABLE ApprovalsExtract_Staging;");
            _databaseService.Execute("TRUNCATE TABLE ApprovalsExtract;");
            _databaseService.Execute("INSERT INTO ApprovalsExtract (ApprenticeshipId, FirstName, Uln) VALUES (123, 'TestName', '123456789');");
            var approvalsExtractInput = new List<ApprovalsExtract>()
            {
                new ApprovalsExtract() { ApprenticeshipId = 123, FirstName = "TestNameUpdated", ULN = "123456789" }
            };

            await _repository.UpsertApprovalsExtractToStaging(approvalsExtractInput);

            // Act
            await _repository.PopulateApprovalsExtract();

            // Assert

            var approvalsExtractOutput = _databaseService.GetList<ApprovalsExtract>("SELECT * FROM ApprovalsExtract;");
            approvalsExtractOutput.Should().NotBeNullOrEmpty();
            approvalsExtractOutput.Should().HaveCount(1);
            approvalsExtractOutput.ElementAt(0).FirstName.Should().Be("TestNameUpdated");
        }

        [Test]
        public async Task When_NotEmpty_Then_NewIsInsertedAndExistingIsUpdated()
        {
            // Arrange
            _databaseService.Execute("TRUNCATE TABLE ApprovalsExtract_Staging;");
            _databaseService.Execute("TRUNCATE TABLE ApprovalsExtract;");
            _databaseService.Execute("INSERT INTO ApprovalsExtract (ApprenticeshipId, FirstName, ULN) VALUES (123, 'TestName', '123456789');");
            var approvalsExtractInput = new List<ApprovalsExtract>()
            {
                new ApprovalsExtract() { ApprenticeshipId = 123, FirstName = "TestNameUpdated", ULN = "123456789" },
                new ApprovalsExtract() { ApprenticeshipId = 456, FirstName = "SecondTestName", ULN = "87654321" }
            };

            await _repository.UpsertApprovalsExtractToStaging(approvalsExtractInput);

            // Act
            await _repository.PopulateApprovalsExtract();


            // Assert

            var approvalsExtractOutput = _databaseService.GetList<ApprovalsExtract>("SELECT * FROM ApprovalsExtract ORDER BY ApprenticeshipId;");
            approvalsExtractOutput.Should().NotBeNullOrEmpty();
            approvalsExtractOutput.Should().HaveCount(2);
            approvalsExtractOutput.ElementAt(0).FirstName.Should().Be("TestNameUpdated");
        }

        [Test]
        public async Task When_ExtractHasAllFieldsPopulated_Then_AllFieldsAreInserted_IntoStaging()
        {
            // Arrange
            _databaseService.Execute("TRUNCATE TABLE ApprovalsExtract_Staging;");
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
                PaymentStatus = 1,
                EmployerAccountId = 100,
                EmployerName = "SFA"
            };
            var approvalsExtractInput = new List<ApprovalsExtract>()
            {
                approvalExtractInput
            };

            // Act

            await _repository.UpsertApprovalsExtractToStaging(approvalsExtractInput);

            // Assert

            var approvalsExtractOutput = _databaseService.GetList<ApprovalsExtract>("SELECT * FROM ApprovalsExtract_Staging;");
            approvalsExtractOutput.Should().NotBeNullOrEmpty();
            approvalsExtractOutput.Should().HaveCount(1);
            approvalsExtractOutput.ElementAt(0).Should().BeEquivalentTo(approvalExtractInput);
        }
    }
}
