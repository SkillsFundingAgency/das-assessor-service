using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories.StandardRepositoryTests
{
    public class MergeStandardsFromStagingTestsFixture
        : FixtureBase<MergeStandardsFromStagingTestsFixture>, IDisposable
    {
        private readonly DatabaseService _databaseService =
            new DatabaseService();

        private readonly SqlConnection _sqlConnection;
        private readonly UnitOfWork _unitOfWork;
        private readonly StandardRepository _sut;

        public DateTime MergeStartedAt { get; private set; }
        public DateTime MergeFinishedAt { get; private set; }

        public MergeStandardsFromStagingTestsFixture()
        {
            _sqlConnection = new SqlConnection(
                _databaseService.SqlConnectionStringTest);

            _unitOfWork = new UnitOfWork(_sqlConnection);
            _sut = new StandardRepository(_unitOfWork);
        }

        public MergeStandardsFromStagingTestsFixture WithStagingStandard(
            StandardModel standard)
        {
            StagingStandardsHandler.InsertRecord(standard);
            return this;
        }

        public MergeStandardsFromStagingTestsFixture WithStandardOption(
            string standardUId,
            string optionName)
        {
            StandardOptionsHandler.InsertRecord(new StandardOptionModel
            {
                StandardUId = standardUId,
                OptionName = optionName
            });

            return this;
        }

        public MergeStandardsFromStagingTestsFixture WithStagingStandardOption(
            string standardUId,
            string optionName)
        {
            StagingStandardOptionsHandler.InsertRecord(new StandardOptionModel
            {
                StandardUId = standardUId,
                OptionName = optionName
            });

            return this;
        }

        public async Task<MergeStandardsFromStagingTestsFixture>
            MergeStandardsFromStaging()
        {
            // Use SQL Server's clock for timestamp assertions.
            MergeStartedAt = Convert.ToDateTime(
                _databaseService.ExecuteScalar("SELECT GETDATE()"));

            _unitOfWork.Begin();

            try
            {
                await _sut.MergeStandardsFromStaging();
                _unitOfWork.Commit();
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }

            MergeFinishedAt = Convert.ToDateTime(
                _databaseService.ExecuteScalar("SELECT GETDATE()"));

            return this;
        }

        public async Task<MergeStandardsFromStagingTestsFixture>
            PrepareStandardsImport()
        {
            _unitOfWork.Begin();

            try
            {
                await _sut.PrepareStandardsImport();
                _unitOfWork.Commit();
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }

            return this;
        }

        public async Task<MergeStandardsFromStagingTestsFixture> ImportStandardsAndOptions(
            IEnumerable<Standard> standards,
            IEnumerable<StandardOption> options)
        {
            _unitOfWork.Begin();

            try
            {
                await _sut.PrepareStandardsImport();
                await _sut.InsertStandardsIntoStaging(standards);
                await _sut.InsertOptionsIntoStaging(options);

                MergeStartedAt = Convert.ToDateTime(
                    _databaseService.ExecuteScalar("SELECT GETDATE()"));

                await _sut.MergeStandardsFromStaging();

                _unitOfWork.Commit();
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }

            MergeFinishedAt = Convert.ToDateTime(
                _databaseService.ExecuteScalar("SELECT GETDATE()"));

            return this;
        }

        public StandardModel GetStandard(string standardUId)
        {
            return StandardsHandler.GetRecords()
                .SingleOrDefault(standard =>
                    standard.StandardUId == standardUId);
        }

        public List<StandardModel> GetStandards()
        {
            return StandardsHandler.GetRecords();
        }

        public List<StandardModel> GetStagingStandards()
        {
            return StagingStandardsHandler.GetRecords();
        }

        public List<StandardOptionModel> GetStandardOptions()
        {
            return _databaseService.GetList<StandardOptionModel>(
                "SELECT [StandardUId], [OptionName] FROM [dbo].[StandardOptions]")
                .ToList();
        }

        public List<StandardOptionModel> GetStagingStandardOptions()
        {
            return StagingStandardOptionsHandler.GetRecords();
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
            _sqlConnection.Dispose();

            base.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}