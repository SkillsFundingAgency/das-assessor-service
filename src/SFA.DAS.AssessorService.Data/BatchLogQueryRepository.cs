using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;

namespace SFA.DAS.AssessorService.Data
{
    public class BatchLogQueryRepository : Repository, IBatchLogQueryRepository
    {
        public BatchLogQueryRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            SqlMapper.AddTypeHandler(typeof(BatchData), new BatchDataHandler());
        }

        public async Task<BatchLog> GetForBatchNumber(int batchNumber)
        {
            var batchLog = await _unitOfWork.Connection.QueryAsync<BatchLog>(
                "SELECT TOP 1 " +
                    "[Id]," + 
                    "[Period], " +
                    "[BatchCreated], " +
                    "[ScheduledDate], " +
                    "[BatchNumber], " +
                    "[NumberOfCertificates], " +
                    "[NumberOfCoverLetters], " +
                    "[CertificatesFileName], " +
                    "[FileUploadStartTime], " +
                    "[FileUploadEndTime], " +
                    "[BatchData]" + 
                "FROM BatchLogs WHERE BatchNumber = @batchNumber",
            param: new { batchNumber},
                transaction: _unitOfWork.Transaction);

            return batchLog.FirstOrDefault();
        }

        public async Task<BatchLog> GetLastBatchLog()
        {
            var results = await _unitOfWork.Connection.QueryAsync<BatchLog>(
                "SELECT TOP 1 " +
                    "[Period], " +
                    "[BatchCreated], " +
                    "[ScheduledDate], " +
                    "[BatchNumber], " +
                    "[NumberOfCertificates], " +
                    "[NumberOfCoverLetters], " +
                    "[CertificatesFileName], " +
                    "[FileUploadStartTime], " +
                    "[FileUploadEndTime], " +
                    "[BatchData]" +
                "FROM BatchLogs ORDER BY BatchCreated DESC",
                transaction: _unitOfWork.Transaction);

            var batchLog = results.FirstOrDefault();

            if (batchLog == null)
            {
                batchLog = new BatchLog
                {
                    BatchNumber = 0
                };
                return batchLog;
            }

            return batchLog;
        }
    }
}