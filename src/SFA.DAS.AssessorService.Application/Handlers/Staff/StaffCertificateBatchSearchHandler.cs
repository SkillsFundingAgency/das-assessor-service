using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Logging;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Staff
{
    public class StaffCertificateBatchSearchHandler : IRequestHandler<StaffBatchSearchRequest, PaginatedList<StaffBatchSearchResult>>
    {
        private readonly IStaffCertificateRepository _staffCertificateRepository;
        private readonly ILogger<StaffCertificateBatchSearchHandler> _logger;

        public StaffCertificateBatchSearchHandler(IStaffCertificateRepository staffCertificateRepository, ILogger<StaffCertificateBatchSearchHandler> logger)
        {
            _staffCertificateRepository = staffCertificateRepository;
            _logger = logger;
        }

        public async Task<PaginatedList<StaffBatchSearchResult>> Handle(StaffBatchSearchRequest request, CancellationToken cancellationToken)
        {
            int pageSize = 10;

            var logs = await _staffCertificateRepository.GetCertificateLogsForBatch(request.BatchNumber);
            int totalRecordCount = logs.Count();

            _logger.LogInformation(logs.Any() ? LoggingConstants.SearchSuccess : LoggingConstants.SearchFailure);

            var logsWeAreInterestedIn = logs.Skip((request.Page - 1) * pageSize).Take(pageSize);

            var searchResults = logsWeAreInterestedIn.Select(sr =>
                    new StaffBatchSearchResult
                    {
                        BatchNumber = sr.BatchNumber,
                        BatchPrintDate = sr.EventTime,
                        Status = sr.Status,
                        CertificateData = JsonConvert.DeserializeObject<CertificateData>(sr.CertificateData),
                        CertificateReference = sr.Certificate?.CertificateReference,
                        Uln = sr.Certificate?.Uln ?? 0,
                        StandardCode = sr.Certificate?.StandardCode ?? 0
                    }).ToList();

            return new PaginatedList<StaffBatchSearchResult>(searchResults, totalRecordCount, request.Page, pageSize);
        }
    }
}
