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
        private readonly ICertificateBatchLogRepository _certificateBatchLogRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateBatchLogReadyToPrintAddCertificatesHandler> _logger;

        public UpdateBatchLogReadyToPrintAddCertificatesHandler(
            ICertificateBatchLogRepository certificateBatchLogRepository,
            ICertificateRepository certificateRepository,
            IUnitOfWork unitOfWork,
            ILogger<UpdateBatchLogReadyToPrintAddCertificatesHandler> logger)
        {
            _certificateBatchLogRepository = certificateBatchLogRepository;
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
                    var updateCertificateBatchLogTask =
                        _certificateBatchLogRepository.UpdateCertificatesReadyToPrintInBatch(certificateIds, request.BatchNumber);

                    var updateCertificateTask =
                        _certificateRepository.UpdateCertificatesReadyToPrintInBatch(certificateIds, request.BatchNumber);

                    await Task.WhenAll(updateCertificateBatchLogTask, updateCertificateTask);
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
