using Dapper;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data
{
    public class ApprovalsExtractRepository : Repository, IApprovalsExtractRepository
    {
        public ApprovalsExtractRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public async Task<DateTime?> GetLatestExtractTimestamp()
        {
            var latestDate = await _unitOfWork.Connection.QueryAsync<DateTime?>(
                @"SELECT MAX(CASE WHEN ISNULL(CreatedOn, 0) > ISNULL(UpdatedOn, 0) THEN CreatedOn ELSE UpdatedOn END) AS MaxDate FROM ApprovalsExtract",
                transaction: _unitOfWork.Transaction);

            return latestDate.FirstOrDefault();
        }

        public void UpsertApprovalsExtract(List<ApprovalsExtract> approvalsExtract)
        {
            if (null == approvalsExtract || !approvalsExtract.Any()) return;

            try
            {
                _unitOfWork.Begin();

                var querySQL = "SELECT * FROM ApprovalsExtract WHERE ApprenticeshipId = @ApprenticeshipId;";
                var insertSQL = @"INSERT INTO ApprovalsExtract (ApprenticeshipId, FirstName, LastName, Uln, TrainingCode, TrainingCourseVersion, TrainingCourseVersionConfirmed, TrainingCourseOption, StandardUid, StartDate, EndDate, CreatedOn, UpdatedOn, StopDate, PauseDate, CompletionDate, UKPRN, LearnRefNumber, PaymentStatus) " +
                                "VALUES (@ApprenticeshipId, @FirstName, @LastName, @Uln, @TrainingCode, @TrainingCourseVersion, @TrainingCourseVersionConfirmed, @TrainingCourseOption, @StandardUid, @StartDate, @EndDate, @CreatedOn, @UpdatedOn, @StopDate, @PauseDate, @CompletionDate, @UKPRN, @LearnRefNumber, @PaymentStatus);";
                var updateSQL = "UPDATE ApprovalsExtract SET FirstName = @FirstName, LastName = @LastName, Uln = @Uln, TrainingCode = @TrainingCode, TrainingCourseVersion = @TrainingCourseVersion, TrainingCourseVersionConfirmed = @TrainingCourseVersionConfirmed, TrainingCourseOption = @TrainingCourseOption, StandardUid = @StandardUid, StartDate = @StartDate, EndDate = @EndDate, CreatedOn = @CreatedOn, UpdatedOn = @UpdatedOn, StopDate = @StopDate, PauseDate = @PauseDate, CompletionDate = @CompletionDate, UKPRN = @UKPRN, LearnRefNumber = @LearnRefNumber, PaymentStatus = @PaymentStatus WHERE ApprenticeshipId = @ApprenticeshipId;";
                foreach (var ae in approvalsExtract)
                {
                    var existingExtract = _unitOfWork.Connection.QueryFirstOrDefault<ApprovalsExtract>(querySQL, new { ApprenticeshipId = ae.ApprenticeshipId }, _unitOfWork.Transaction);
                    if(null == existingExtract)
                    {
                        _unitOfWork.Connection.Execute(insertSQL, ae, _unitOfWork.Transaction);
                    }
                    else
                    {
                        _unitOfWork.Connection.Execute(updateSQL, ae, _unitOfWork.Transaction);
                    }
                }

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw ex;
            }
        }

        public async Task<int> PopulateLearner()
        {
            try
            {
                var result = await _unitOfWork.Connection.ExecuteScalarAsync<int>("PopulateLearner", commandType: CommandType.StoredProcedure, commandTimeout: 300);
                if (0 != result)
                {
                    throw new Exception("Stored procedure PopulateLearner failed to complete successfully.");
                }
                int rowCount = await _unitOfWork.Connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM Learner");
                return rowCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
