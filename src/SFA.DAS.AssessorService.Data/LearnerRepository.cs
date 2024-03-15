using Dapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data
{
    public class LearnerRepository : Repository, ILearnerRepository
    {
        public LearnerRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            SqlMapper.AddTypeHandler(typeof(SearchData), new SearchDataHandler());
        }

        public async Task<IEnumerable<Learner>> SearchForLearnerByUln(long uln)
        {
            var continuing = (int)CompletionStatus.Continuing;
            var complete = (int)CompletionStatus.Complete;

            return (await _unitOfWork.Connection.QueryAsync<Learner>(
               $@"SELECT * FROM [Learner] WHERE [Uln] = @uln AND [CompletionStatus] IN (@continuing, @complete)",
                 param: new { uln, continuing, complete },
                 transaction: _unitOfWork.Transaction)).ToList();
        }

        public async Task<Learner> Get(long uln, int stdCode)
        {
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Learner>(
                @"SELECT * FROM [Learner] WHERE [Uln] = @uln AND [StdCode] = @stdCode",
                  param: new { uln, stdCode },
                  transaction: _unitOfWork.Transaction);
        }

        public async Task<ApprenticeLearner> Get(long apprenticeshipId)
        {
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<ApprenticeLearner>(
                @"SELECT
                    l.ApprenticeshipId,
                    l.UkPrn,
                    l.LearnStartDate,
                    l.PlannedEndDate,
                    l.StdCode,
                    l.StandardUId,
                    l.StandardReference,
                    l.StandardName,
                    l.CompletionStatus,
                    l.ApprovalsStopDate,
                    l.ApprovalsPauseDate,
                    l.EstimatedEndDate,
                    l.Uln,
                    l.GivenNames,
                    l.FamilyName,
                    l.LearnActEndDate,
                    l.IsTransfer,
                    l.DateTransferIdentified,
                    p.Name as ProviderName
                FROM [Learner] l 
                    LEFT JOIN [Providers] p on l.UkPrn = p.Ukprn                
                WHERE [ApprenticeshipId] = @apprenticeshipId",
                param: new { apprenticeshipId },
                transaction: _unitOfWork.Transaction);
        }


        public async Task<int> GetEpaoPipelinesCount(string epaOrgId, int? stdCode, int pipelineCutoff)
        {
            var result = await _unitOfWork.Connection.QueryAsync<int>(
                "GetEPAO_Pipelines_Count",
                param: new
                {
                    epaOrgId,
                    stdCode,
                    pipelineCutoff
                },
                transaction: _unitOfWork.Transaction,
                commandType: CommandType.StoredProcedure);

            return result.First();
        }

        public async Task StoreSearchLog(SearchLog log)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                @"INSERT INTO SearchLogs (Surname, Uln, SearchTime, SearchData, NumberOfResults, Username) 
                  VALUES (@surname, @uln, @searchTime, @searchData, @numberOfResults, @username)",
                  param: new { log.Surname, log.Uln, log.SearchTime, log.SearchData, log.NumberOfResults, log.Username },
                  transaction: _unitOfWork.Transaction);
        }
    }
}