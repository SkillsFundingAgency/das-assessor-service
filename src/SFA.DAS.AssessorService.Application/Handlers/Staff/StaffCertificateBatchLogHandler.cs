using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Logging;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Staff
{
    public class StaffCertificateBatchLogHandler : IRequestHandler<StaffBatchLogRequest, PaginatedList<StaffBatchLogResult>>
    {
        private readonly IStaffCertificateRepository _staffCertificateRepository;
        private readonly ILogger<StaffCertificateBatchLogHandler> _logger;

        public StaffCertificateBatchLogHandler(IStaffCertificateRepository staffCertificateRepository, ILogger<StaffCertificateBatchLogHandler> logger)
        {
            _staffCertificateRepository = staffCertificateRepository;
            _logger = logger;
        }

        public async Task<PaginatedList<StaffBatchLogResult>> Handle(StaffBatchLogRequest request, CancellationToken cancellationToken)
        {
            int pageSize = 10;

            var result = await _staffCertificateRepository.GetBatchLogs(request.Page, pageSize);

            _logger.LogInformation(result.PageOfResults.Any() ? LoggingConstants.SearchSuccess : LoggingConstants.SearchFailure);

            var searchResults = result.PageOfResults.Select(sr =>
                    new StaffBatchLogResult
                    {
                        BatchNumber = sr.BatchNumber,
                        ScheduledDate = sr.ScheduledDate,
                        NumberOfCertificates = sr.NumberOfCertificates,
                        NumberOfCoverLetters = sr.NumberOfCoverLetters
                    }).ToList();

            return new PaginatedList<StaffBatchLogResult>(searchResults, result.TotalCount, request.Page, pageSize);
        }
    }
}
