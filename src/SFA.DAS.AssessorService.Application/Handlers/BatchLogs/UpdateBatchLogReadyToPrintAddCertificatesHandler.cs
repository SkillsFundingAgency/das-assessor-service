using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.BatchLogs
{
    public class UpdateBatchLogReadyToPrintAddCertificatesHandler : IRequestHandler<UpdateBatchLogReadyToPrintAddCertificatesRequest, int>
    {
        private readonly IBatchLogRepository _batchLogRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateBatchLogReadyToPrintAddCertificatesHandler> _logger;

        public UpdateBatchLogReadyToPrintAddCertificatesHandler(
            IBatchLogRepository batchLogRepository,
            ICertificateRepository certificateRepository,
            IUnitOfWork unitOfWork,
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
                _unitOfWork.Begin();

                var excludedOverallGrades = new[] { "Fail" };
                var includedStatus = new[] { "Submitted", "Reprint" };

                var certificateIds = await _certificateRepository.GetCertificatesReadyToPrint(
                    request.MaxCertificatesToBeAdded, 
                    excludedOverallGrades, 
                    includedStatus);

                if (certificateIds.Length > 0)
                {
                    await _batchLogRepository
                        .UpsertCertificatesReadyToPrintInBatch(request.BatchNumber, certificateIds);

                    await _certificateRepository.
                        UpdateCertificatesReadyToPrintInBatch(certificateIds, request.BatchNumber);
                }

                _unitOfWork.Commit();

                return certificateIds.Length;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Failed to add ready to print certificates to batch {request.BatchNumber}");
                _unitOfWork.Rollback();
                throw;
            }
        }
    }
}
