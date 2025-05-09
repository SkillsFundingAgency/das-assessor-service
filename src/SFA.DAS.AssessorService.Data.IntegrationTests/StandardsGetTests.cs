﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    [TestFixture]
    public class StandardsGetTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();

        private SqlConnection _databaseConnection;
        private UnitOfWork _unitOfWork;
        private StandardRepository _repository;

        private List<StandardModel> _standards;

        [OneTimeSetUp]
        public void SetupStandardsTable()
        {
            _databaseConnection = new SqlConnection(_databaseService.SqlConnectionStringTest);
            _unitOfWork = new UnitOfWork(_databaseConnection);
            _repository = new StandardRepository(_unitOfWork);
            
            _standards = GetListOfStandardVersions();

            StandardsHandler.InsertRecords(_standards);
        }

        [TestCase("ST0001", "1.0", "ST0001_1.0")]
        [TestCase("ST0001", "", "ST0001_1.12")]
        [TestCase("ST0001", "1.10", "ST0001_1.10")]
        [TestCase("ST0001", "1.12", "ST0001_1.12")]
        public async Task GetStandardByStandardReferenceAndVersion_ReturnsCorrectStandard(string standardReference, string version, string standardUId)
        {
            var expectedStandard = _standards.Single(s => s.StandardUId == standardUId);

            var standard = await _repository.GetStandardVersionByIFateReferenceNumber(standardReference, version);

            standard.StandardUId.Should().Be(expectedStandard.StandardUId);
        }

        [OneTimeTearDown]
        public void TearDownStandardsTable()
        {
            StandardsHandler.DeleteAllRecords();

            if (_databaseConnection != null)
            {
                _databaseConnection.Dispose();
            }
        }

        private List<StandardModel> GetListOfStandardVersions()
        {
            return new List<StandardModel>{
                new StandardModel
                {
                    StandardUId = "ST0001_1.0",
                    IFateReferenceNumber = "ST0001",
                    Version = "1.0",
                    VersionMajor = 1,
                    VersionMinor = 0,
                    Title = "Standard",
                    Level = 4,
                    Status = "Active",
                    IsActive = 1,
                    MaxFunding = 10000,
                    TypicalDuration = 12,
                    ProposedMaxFunding = 10000,
                    ProposedTypicalDuration = 12,
                    EPAChanged = false,
                    VersionApprovedForDelivery = DateTime.Now.AddMonths(-3)
                },
                new StandardModel
                {
                    StandardUId = "ST0001_1.1",
                    IFateReferenceNumber = "ST0001",
                    Version = "1.1",
                    VersionMajor = 1,
                    VersionMinor = 1,
                    Title = "Standard",
                    Level = 4,
                    Status = "Active",
                    IsActive = 1,
                    MaxFunding = 10000,
                    TypicalDuration = 12,
                    ProposedMaxFunding = 10000,
                    ProposedTypicalDuration = 12,
                    EPAChanged = false,
                    VersionApprovedForDelivery = DateTime.Now.AddMonths(-3)
                },
                 new StandardModel
                {
                    StandardUId = "ST0001_1.2",
                    IFateReferenceNumber = "ST0001",
                    Version = "1.2",
                    VersionMajor = 1,
                    VersionMinor = 2,
                    Title = "Standard",
                    Level = 4,
                    Status = "Active",
                    IsActive = 1,
                    MaxFunding = 10000,
                    TypicalDuration = 12,
                    ProposedMaxFunding = 10000,
                    ProposedTypicalDuration = 12,
                    EPAChanged = false,
                    VersionApprovedForDelivery = DateTime.Now.AddMonths(-3)
                },
                new StandardModel
                {
                    StandardUId = "ST0001_1.10",
                    IFateReferenceNumber = "ST0001",
                    Version = "1.10",
                    VersionMajor = 1,
                    VersionMinor = 10,
                    Title = "Standard",
                    Level = 4,
                    Status = "Active",
                    IsActive = 1,
                    MaxFunding = 10000,
                    TypicalDuration = 12,
                    ProposedMaxFunding = 10000,
                    ProposedTypicalDuration = 12,
                    EPAChanged = false,
                    VersionApprovedForDelivery = DateTime.Now.AddMonths(-3)
                },
                new StandardModel
                {
                    StandardUId = "ST0001_1.12",
                    IFateReferenceNumber = "ST0001",
                    Version = "1.12",
                    VersionMajor = 1,
                    VersionMinor = 12,
                    Title = "Standard",
                    Level = 4,
                    Status = "Active",
                    IsActive = 1,
                    MaxFunding = 10000,
                    TypicalDuration = 12,
                    ProposedMaxFunding = 10000,
                    ProposedTypicalDuration = 12,
                    EPAChanged = false,
                    VersionApprovedForDelivery = DateTime.Now.AddMonths(-3)
                }
            };
        }
    }
}
