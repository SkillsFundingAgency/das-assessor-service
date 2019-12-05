using Dapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data
{
    public class IlrRepository : Repository, IIlrRepository
    {
        public IlrRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<Ilr>> SearchForLearnerByUln(long uln)
        {
            return (await _unitOfWork.Connection.QueryAsync<Ilr>(
                @"SELECT * FROM Ilrs WHERE [Uln] = @uln AND [CompletionStatus] IN (1, 2)",
                  param: new { uln },
                  transaction: _unitOfWork.Transaction)).ToList();
        }

        public async Task<Ilr> Get(long uln, int stdCode)
        {
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Ilr>(
                @"SELECT * FROM Ilrs WHERE [Uln] = @uln AND [StdCode] = @stdCode",
                  param: new { uln, stdCode },
                  transaction: _unitOfWork.Transaction);
        }

        public async Task StoreSearchLog(SearchLog log)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                @"INSERT INTO SearchLogs (Surname, Uln, SearchTime, SearchData, NumberOfResults, Username) 
                  VALUES (@surname, @uln, @searchTime, @searchData, @numberOfResults, @username)", 
                  param: new { log.Surname, log.Uln, log.SearchTime, log.SearchData, log.NumberOfResults, log.Username },
                  transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Ilr>> Search(string searchQuery)
        {            
            return (await _unitOfWork.Connection.QueryAsync<Ilr>(
                @"SELECT * FROM Ilrs WHERE [FamilyName] = @searchQuery OR [GivenNames] = @searchQuery OR [Uln] = CAST(bigint, @searchQuery)",
                  param: new { searchQuery },
                  transaction: _unitOfWork.Transaction)).ToList();
        }

        public async Task Create(string source, long ukprn, long uln, int stdCode, int? fundingModel, string givenNames, string familyName,
                string epaOrgId, DateTime? learnStartDate, DateTime? plannedEndDate, int? completionStatus, string learnRefNumber, string delLocPostCode,
                DateTime? learnActEndDate, int? withdrawReason, int? outcome, DateTime? achDate, string outGrade)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                @"INSERT INTO [Ilrs] (Uln, GivenNames, FamilyName, UkPrn, StdCode, LearnStartDate, EpaOrgId, FundingModel, 
                  ApprenticeshipId, EmployerAccountId, Source, CreatedAt, UpdatedAt, LearnRefNumber, CompletionStatus, EventId, PlannedEndDate, DelLocPostCode, 
                  LearnActEndDate, WithdrawReason, Outcome, AchDate, OutGrade) 
                  VALUES (@uln, @givenNames, @familyName, @ukprn, @stdCode, @learnStartDate, @epaOrgId, @fundingModel, null, null,
                        @source, @createdAt, null, @learnRefNumber, @completionStatus, null, @plannedEndDate, @delLocPostCode, 
                        @learnActEndDate, @withdrawReason, @outcome, @achDate, @outGrade )",
                  param: new
                  {
                      uln,
                      givenNames,
                      familyName,
                      ukprn,
                      stdCode,
                      learnStartDate,
                      epaOrgId,
                      fundingModel,
                      source,
                      createdAt = DateTime.UtcNow,
                      learnRefNumber,
                      completionStatus,
                      plannedEndDate,
                      delLocPostCode,
                      learnActEndDate,
                      withdrawReason,
                      outcome,
                      achDate,
                      outGrade
                  },
                  transaction: _unitOfWork.Transaction);
        }

        public async Task Update(string source, long ukprn, long uln, int stdCode, int? fundingModel, string givenNames, string familyName,
                string epaOrgId, DateTime? learnStartDate, DateTime? plannedEndDate, int? completionStatus, string learnRefNumber, string delLocPostCode,
                DateTime? learnActEndDate, int? withdrawReason, int? outcome, DateTime? achDate, string outGrade)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                @"UPDATE [Ilrs] SET [GivenNames] = @givenNames, [FamilyName] = @familyName, [UkPrn] = @ukprn, 
                                    [LearnStartDate] = @learnStartDate, [EpaOrgId] = @epaOrgId, [FundingModel] = @fundingModel,
                                    [ApprenticeshipId] = null, [EmployerAccountId] = null, [Source] = @source, [CreatedAt] = [CreatedAt], [UpdatedAt] = @updatedAt,
                                    [LearnRefNumber] = @learnRefNumber, [CompletionStatus] = @completionStatus, [EventId] = null, [PlannedEndDate] = @plannedEndDate, 
                                    [DelLocPostCode] = @delLocPostCode, [LearnActEndDate] = @learnActEndDate, [WithdrawReason] = @withdrawReason, 
                                    [Outcome] = @outcome, [AchDate] = @achDate, [OutGrade] = @outGrade
                                WHERE
                                    [Uln] = @uln AND [StdCode] = @stdCode",
                  param: new
                  {
                      uln,
                      givenNames,
                      familyName,
                      ukprn,
                      stdCode,
                      learnStartDate,
                      epaOrgId,
                      fundingModel,
                      source,
                      updatedAt = DateTime.UtcNow,
                      learnRefNumber,
                      completionStatus,
                      plannedEndDate,
                      delLocPostCode,
                      learnActEndDate,
                      withdrawReason,
                      outcome,
                      achDate,
                      outGrade
                  },
                  transaction: _unitOfWork.Transaction);
        }
    }
}