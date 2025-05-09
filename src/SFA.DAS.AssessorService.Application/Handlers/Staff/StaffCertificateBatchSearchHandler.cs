﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Application.Logging;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Handlers.Staff
{
    public class StaffCertificateBatchSearchHandler : IRequestHandler<StaffBatchSearchRequest, StaffBatchSearchResponse>
    {
        private readonly IStaffCertificateRepository _staffCertificateRepository;
        private readonly ILogger<StaffCertificateBatchSearchHandler> _logger;

        public StaffCertificateBatchSearchHandler(IStaffCertificateRepository staffCertificateRepository, ILogger<StaffCertificateBatchSearchHandler> logger)
        {
            _staffCertificateRepository = staffCertificateRepository;
            _logger = logger;
        }

        public async Task<StaffBatchSearchResponse> Handle(StaffBatchSearchRequest request, CancellationToken cancellationToken)
        {
            int pageSize = 10;

            var result = await _staffCertificateRepository.GetCertificateLogsForBatch(request.BatchNumber, request.Page, pageSize);

            _logger.LogInformation(result.PageOfResults.Any() ? LoggingConstants.SearchSuccess : LoggingConstants.SearchFailure);

            var searchResults = result.PageOfResults.Select(sr =>
                    new StaffBatchSearchResult
                    {
                        BatchNumber = sr.BatchNumber,
                        StatusAt = sr.StatusAt,
                        Status = sr.Status,
                        CertificateData = JsonConvert.DeserializeObject<CertificateData>(sr.CertificateData),
                        CertificateReference = sr.CertificateReference,
                        Uln = sr.Uln,
                        StandardCode = sr.StandardCode,
                        FrameworkLearnerId = sr.FrameworkLearnerId,
                    }).ToList();

            return new StaffBatchSearchResponse
            {
                SentToPrinterDate = result.SentToPrinterAt,
                PrintedDate = result.PrintedAt,
                Results = new PaginatedList<StaffBatchSearchResult>(searchResults, result.TotalCount, request.Page, pageSize)
            };
        }
    }
}
