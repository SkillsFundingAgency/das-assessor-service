using Dapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data
{
    public class IlrRepository : Repository, IIlrRepository
    {
        public IlrRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        { }
        public async Task<Ilr> Get(long uln, int stdCode)
        {
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Ilr>(
                @"SELECT * FROM [Ilrs] WHERE [Uln] = @uln AND [StdCode] = @stdCode",
                  param: new { uln, stdCode },
                  transaction: _unitOfWork.Transaction);
        }

        public async Task Create(Ilr ilr)
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
                      ilr.Uln,
                      ilr.GivenNames,
                      ilr.FamilyName,
                      ilr.UkPrn,
                      ilr.StdCode,
                      ilr.LearnStartDate,
                      ilr.EpaOrgId,
                      ilr.FundingModel,
                      ilr.Source,
                      createdAt = DateTime.UtcNow,
                      ilr.LearnRefNumber,
                      ilr.CompletionStatus,
                      ilr.PlannedEndDate,
                      ilr.DelLocPostCode,
                      ilr.LearnActEndDate,
                      ilr.WithdrawReason,
                      ilr.Outcome,
                      ilr.AchDate,
                      ilr.OutGrade
                  },
                  transaction: _unitOfWork.Transaction);
        }

        public async Task Update(Ilr ilr)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                @"MERGE INTO Ilrs  
                  USING (
                  -- only need to update when the data values have changed
                  SELECT [Uln], [StdCode], [GivenNames], [FamilyName], [UkPrn], [LearnStartDate], [EpaOrgId], [FundingModel], [Source], [LearnRefNumber], 
                         [CompletionStatus], [PlannedEndDate], [DelLocPostCode], [LearnActEndDate], [WithdrawReason] , [Outcome], [AchDate], [OutGrade]
                    FROM Ilrs
                   WHERE [Uln] = @Uln AND [StdCode] = @StdCode
                  EXCEPT
                  SELECT @Uln, @StdCode, @GivenNames, @FamilyName, @UkPrn, @LearnStartDate, @EpaOrgId, @FundingModel, @Source, @LearnRefNumber, 
                         @CompletionStatus, @PlannedEndDate, @DelLocPostCode, @LearnActEndDate, @WithdrawReason , @Outcome, @AchDate, @OutGrade
                        ) upd     
                  ON ( Ilrs.[Uln] = upd.[Uln] AND Ilrs.[StdCode] = upd.[StdCode] )
                WHEN MATCHED THEN
                UPDATE SET [GivenNames] = @givenNames, [FamilyName] = @familyName, [UkPrn] = @ukprn, 
                           [LearnStartDate] = @learnStartDate, [EpaOrgId] = @epaOrgId, [FundingModel] = @fundingModel,
                           [ApprenticeshipId] = null, [EmployerAccountId] = null, [Source] = @source, [UpdatedAt] = @updatedAt,
                           [LearnRefNumber] = @learnRefNumber, [CompletionStatus] = @completionStatus, [EventId] = null, [PlannedEndDate] = @plannedEndDate, 
                           [DelLocPostCode] = @delLocPostCode, [LearnActEndDate] = @learnActEndDate, [WithdrawReason] = @withdrawReason, 
                           [Outcome] = @outcome, [AchDate] = @achDate, [OutGrade] = @outGrade ;",
                  param: new
                  {
                      ilr.Uln,
                      ilr.GivenNames,
                      ilr.FamilyName,
                      ilr.UkPrn,
                      ilr.StdCode,
                      ilr.LearnStartDate,
                      ilr.EpaOrgId,
                      ilr.FundingModel,
                      ilr.Source,
                      updatedAt = DateTime.UtcNow,
                      ilr.LearnRefNumber,
                      ilr.CompletionStatus,
                      ilr.PlannedEndDate,
                      ilr.DelLocPostCode,
                      ilr.LearnActEndDate,
                      ilr.WithdrawReason,
                      ilr.Outcome,
                      ilr.AchDate,
                      ilr.OutGrade
                  },
                  transaction: _unitOfWork.Transaction);
        }
    }
}