using System;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IBatchLogRepository
    {
        Task<BatchLog> Create(BatchLog batchLog);
        Task<ValidationResponse> UpdateBatchLogSentToPrinter(int batchNumber, DateTime batchCreated, int numberOfCertificates, 
            int numberOfCoverLetters, string certificatesFileName, DateTime fileUploadStartTime, DateTime fileUploadEndTime, BatchData batchData);
        Task<ValidationResponse> UpdateBatchLogPrinted(int batchNumber, BatchData batchData);
    }
}