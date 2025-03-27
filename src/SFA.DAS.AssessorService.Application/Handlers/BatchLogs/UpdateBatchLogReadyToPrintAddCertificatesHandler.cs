using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.BatchLogs
{
    public class UpdateBatchLogReadyToPrintAddCertificatesHandler : IRequestHandler<UpdateBatchLogReadyToPrintAddCertificatesRequest, int>
    {
        private readonly IBatchLogRepository _batchLogRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly IAssessorUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateBatchLogReadyToPrintAddCertificatesHandler> _logger;

        public UpdateBatchLogReadyToPrintAddCertificatesHandler(
            IBatchLogRepository batchLogRepository,
            ICertificateRepository certificateRepository,
            IAssessorUnitOfWork unitOfWork,
            ILogger<UpdateBatchLogReadyToPrintAddCertificatesHandler> logger)
        {
            _batchLogRepository = batchLogRepository;
            _certificateRepository = certificateRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> Handle(UpdateBatchLogReadyToPrintAddCertificatesRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var excludedOverallGrades = new[] { "Fail" };
                    var includedStatus = new[] { "Submitted", "Reprint" };

                    var certificateIds = await _certificateRepository.GetCertificatesReadyToPrint(
                        request.MaxCertificatesToBeAdded,
                        excludedOverallGrades,
                        includedStatus);

                    if (certificateIds.Length > 0)
                    {
                        await _batchLogRepository
                            .UpsertCertificatesReadyToPrintInBatch(certificateIds, request.BatchNumber);

                        await _certificateRepository.
                            UpdateCertificatesReadyToPrintInBatch(certificateIds, request.BatchNumber);
                    }

                    return certificateIds.Length;
                });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Failed to add ready to print certificates to batch {request.BatchNumber}");
                throw;
            }
        }
    }
}
