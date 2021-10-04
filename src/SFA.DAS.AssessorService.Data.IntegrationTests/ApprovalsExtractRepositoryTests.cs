using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.JsonData;
using Newtonsoft.Json;
using FluentAssertions;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    [TestFixture]
    public class ApprovalsExtractRepositoryTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private AssessorDbContext _context;
        private UnitOfWork _unitOfWork;
        private ApprovalsExtractRepository _repository;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var option = new DbContextOptionsBuilder<AssessorDbContext>();
            option.UseSqlServer(_databaseService.WebConfiguration.SqlConnectionString, options => options.EnableRetryOnFailure(3));

            _context = new AssessorDbContext(option.Options);
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
    }
}
